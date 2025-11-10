import os
import json
import asyncio
from typing import List, Dict, Any, Optional
from pathlib import Path
import chardet
from embeddings import EmbeddingsClient
from document_parser import DocumentParser
from converter import DocumentConverter

class DocumentIndexer:
    def __init__(self, config_manager=None, use_native_embedding: bool = False, gguf_model: str = None):
        self.config_manager = config_manager
        self.embeddings_client = EmbeddingsClient(
            config_manager=config_manager,
            use_native_embedding=use_native_embedding,
            gguf_model=gguf_model
        )
        self.document_parser = DocumentParser()
        self.converter = DocumentConverter()  # Add MarkItDown converter
        self.supported_extensions = {
            # Text and code files
            '.txt', '.md', '.py', '.js', '.ts', '.jsx', '.tsx',
            '.json', '.yaml', '.yml', '.toml', '.ini', '.cfg',
            '.html', '.css', '.xml', '.csv', '.sql', '.sh',
            '.bat', '.ps1', '.c', '.cpp', '.h', '.java', '.cs',
            '.go', '.rs', '.rb', '.php', '.swift', '.kt', '.scala',
            # Office documents
            '.pdf', '.docx', '.xlsx', '.pptx'
        }

        # Load chunk settings from config if available
        if config_manager:
            self.chunk_size = config_manager.get("rag_settings.chunk_size", 500)
            self.chunk_overlap = config_manager.get("rag_settings.chunk_overlap", 100)
        else:
            self.chunk_size = 500
            self.chunk_overlap = 100

        self.active_tasks = {}  # Store active indexing tasks

    def detect_encoding(self, file_path: str) -> str:
        with open(file_path, 'rb') as f:
            raw_data = f.read()
            result = chardet.detect(raw_data)
            return result['encoding'] or 'utf-8'

    def read_file(self, file_path: str) -> Optional[Path]:
        """Convert file to markdown using MarkItDown."""
        try:
            file_path_obj = Path(file_path)

            # Convert to markdown using MarkItDown
            md_path = self.converter.convert_to_markdown(file_path_obj)
            if md_path:
                print(f"Successfully converted file to markdown", flush=True)
            else:
                print(f"Failed to convert file to markdown", flush=True)
            return md_path
        except Exception as e:
            print(f"Error converting file: {e}", flush=True)
            import traceback
            traceback.print_exc()
            return None

    def chunk_text(self, text: str, file_path: str) -> List[Dict[str, Any]]:
        import re
        # Split by spaces, tabs, and Japanese punctuation marks
        words = re.split(r'[ \t\n\r、。]+', text)
        # Remove empty strings from the list
        words = [w for w in words if w]
        chunks = []

        if len(words) <= self.chunk_size:
            return [{
                "text": text,
                "file": file_path,
                "chunk_index": 0
            }]

        for i in range(0, len(words), self.chunk_size - self.chunk_overlap):
            chunk_words = words[i:i + self.chunk_size]
            chunk_text = " ".join(chunk_words)

            chunks.append({
                "text": chunk_text,
                "file": file_path,
                "chunk_index": len(chunks)
            })

        return chunks

    def get_files_to_index(self, folder_path: str) -> List[str]:
        files_to_index = []

        for root, _, files in os.walk(folder_path):
            for file in files:
                file_path = os.path.join(root, file)
                file_ext = Path(file).suffix.lower()

                if file_ext in self.supported_extensions:
                    files_to_index.append(file_path)

        return files_to_index

    async def index_folder(self, folder_path: str, force_reindex: bool = False, task_id: str = None) -> Dict[str, Any]:
        vector_file = os.path.join(folder_path, "embeddings.jsonl")
        metadata_file = os.path.join(folder_path, ".index_metadata.json")
        
        metadata = {}
        if os.path.exists(metadata_file) and not force_reindex:
            try:
                with open(metadata_file, 'r', encoding='utf-8') as f:
                    metadata = json.load(f)
            except (json.JSONDecodeError, FileNotFoundError):
                metadata = {}

        all_files = self.get_files_to_index(folder_path)
        
        if not all_files and not metadata:
            return {
                "status": "no_files",
                "message": "No supported files found and no existing index.",
                "supported_extensions": list(self.supported_extensions)
            }

        files_on_disk = {os.path.relpath(f, folder_path) for f in all_files}
        files_in_metadata = set(metadata.keys())

        files_to_process = []
        files_to_keep = []

        # Identify new and modified files
        print("[DEBUG] indexer.py: Identifying files to process/keep", flush=True)
        for file_rel_path in files_on_disk:
            file_abs_path = os.path.join(folder_path, file_rel_path)
            entry = metadata.get(file_rel_path)
            
            md_path = Path(file_abs_path).with_suffix('.md')
            
            if entry:
                print(f"[DEBUG] indexer.py: Checking file {file_rel_path}", flush=True)
                print(f"[DEBUG] indexer.py: entry.chunk_size = {entry.get('chunk_size')}, self.chunk_size = {self.chunk_size}", flush=True)
                print(f"[DEBUG] indexer.py: entry.chunk_overlap = {entry.get('chunk_overlap')}, self.chunk_overlap = {self.chunk_overlap}", flush=True)
                print(f"[DEBUG] indexer.py: entry.original_mtime = {entry.get('original_mtime')}, file_mtime = {os.path.getmtime(file_abs_path)}", flush=True)
                if md_path.exists():
                    print(f"[DEBUG] indexer.py: entry.md_mtime = {entry.get('md_mtime')}, md_file_mtime = {os.path.getmtime(md_path)}", flush=True)

            if not entry or \
               entry.get('chunk_size') != self.chunk_size or \
               entry.get('chunk_overlap') != self.chunk_overlap or \
               entry.get('original_mtime') != os.path.getmtime(file_abs_path) or \
               (md_path.exists() and entry.get('md_mtime') != os.path.getmtime(md_path)):
                print(f"[DEBUG] indexer.py: Processing {file_rel_path}", flush=True)
                files_to_process.append(file_abs_path)
            else:
                files_to_keep.append(file_rel_path)
                print(f"[INFO] Skipping embedding for {file_rel_path}, already up to date.", flush=True)

        deleted_files = files_in_metadata - files_on_disk

        if not files_to_process and not deleted_files and os.path.exists(vector_file):
            return {
                "status": "success",
                "message": "All files are up to date.",
                "files_indexed": len(files_on_disk),
                "chunks_created": sum(m.get('num_chunks', 0) for m in metadata.values()),
                "vector_file": vector_file
            }

        # Keep existing vectors for unchanged files
        kept_vectors = []
        if os.path.exists(vector_file):
            with open(vector_file, 'r', encoding='utf-8') as f:
                for line in f:
                    try:
                        vector = json.loads(line)
                        if vector.get('file') in files_to_keep:
                            kept_vectors.append(vector)
                    except json.JSONDecodeError:
                        continue
        
        # Process new and modified files
        new_vectors = []
        new_metadata = {}

        if files_to_process:
            markdown_files = []
            for file_path in files_to_process:
                md_path = self.read_file(file_path)
                if md_path:
                    markdown_files.append((file_path, md_path))

            all_chunks = []
            for original_path, md_path in markdown_files:
                try:
                    with open(md_path, 'r', encoding='utf-8') as f:
                        content = f.read()
                    relative_path = os.path.relpath(original_path, folder_path)
                    chunks = self.chunk_text(content, relative_path)
                    all_chunks.extend(chunks)
                    
                    new_metadata[relative_path] = {
                        "original_mtime": os.path.getmtime(original_path),
                        "md_mtime": os.path.getmtime(md_path),
                        "chunk_size": self.chunk_size,
                        "chunk_overlap": self.chunk_overlap,
                        "num_chunks": len(chunks)
                    }
                except Exception as e:
                    print(f"Error processing file {original_path}: {e}")

            batch_size = 10
            for i in range(0, len(all_chunks), batch_size):
                batch = all_chunks[i:i + batch_size]
                texts = [chunk["text"] for chunk in batch]
                try:
                    embeddings = await self.embeddings_client.get_embeddings(texts)
                    for chunk, embedding in zip(batch, embeddings):
                        new_vectors.append({
                            "text": chunk["text"],
                            "file": chunk["file"],
                            "chunk_index": chunk["chunk_index"],
                            "embedding": embedding
                        })
                except Exception as e:
                    print(f"Error generating embeddings for batch: {e}")

        # Combine vectors and write to file
        final_vectors = kept_vectors + new_vectors
        with open(vector_file, 'w', encoding='utf-8') as f:
            for vector in final_vectors:
                f.write(json.dumps(vector, ensure_ascii=False) + '\n')

        # Update metadata
        final_metadata = {k: v for k, v in metadata.items() if k in files_to_keep}
        final_metadata.update(new_metadata)
        with open(metadata_file, 'w', encoding='utf-8') as f:
            json.dump(final_metadata, f, indent=2)

        return {
            "status": "success",
            "message": "Indexing completed successfully.",
            "files_indexed": len(final_metadata),
            "chunks_created": len(final_vectors),
            "vector_file": vector_file
        }