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
    def __init__(self, config_manager=None):
        self.config_manager = config_manager
        self.embeddings_client = EmbeddingsClient(config_manager=config_manager)
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

        if os.path.exists(vector_file) and not force_reindex:
            return {
                "status": "exists",
                "message": "Vector index already exists. Use force_reindex=True to recreate.",
                "vector_file": vector_file
            }

        files_to_index = self.get_files_to_index(folder_path)

        if not files_to_index:
            return {
                "status": "no_files",
                "message": "No supported files found in the specified folder.",
                "supported_extensions": list(self.supported_extensions)
            }

        # Update progress
        if task_id and task_id in self.active_tasks:
            self.active_tasks[task_id]["progress"] = {
                "stage": "converting_to_markdown",
                "files_processed": 0,
                "total_files": len(files_to_index),
                "chunks_created": 0
            }

        # Step 1: Convert all files to markdown
        print(f"Starting indexing for {folder_path} with {len(files_to_index)} files")
        print(f"Chunk size: {self.chunk_size}, Chunk overlap: {self.chunk_overlap}")
        markdown_files = []
        seen_base_names = set()  # Track base filenames to avoid duplicates

        for idx, file_path in enumerate(files_to_index):
            # Check if cancelled
            if task_id and task_id in self.active_tasks:
                if self.active_tasks[task_id].get("cancelled"):
                    return {
                        "status": "cancelled",
                        "message": "Indexing was cancelled during conversion",
                        "files_processed": idx,
                        "total_files": len(files_to_index)
                    }
                # Update progress
                self.active_tasks[task_id]["progress"]["files_processed"] = idx + 1

            # Skip if we've already processed this base file (avoid PDF/MD duplicates)
            file_base = Path(file_path).stem
            if file_base in seen_base_names:
                print(f"Skipping duplicate file (already processed)", flush=True)
                continue

            md_path = self.read_file(file_path)  # Now returns markdown path
            if md_path:
                markdown_files.append((file_path, md_path))
                seen_base_names.add(file_base)

        # Step 2: Process markdown files and create chunks
        if task_id and task_id in self.active_tasks:
            self.active_tasks[task_id]["progress"]["stage"] = "chunking_documents"

        all_chunks = []
        for original_path, md_path in markdown_files:
            try:
                with open(md_path, 'r', encoding='utf-8') as f:
                    content = f.read()
                    print(f"Read {len(content)} chars from markdown file", flush=True)

                relative_path = os.path.relpath(original_path, folder_path)
                chunks = self.chunk_text(content, relative_path)
                print(f"Created {len(chunks)} chunks", flush=True)
                all_chunks.extend(chunks)
            except Exception as e:
                print(f"Error reading markdown file {md_path}: {e}")

        vectors = []
        batch_size = 10
        total_batches = (len(all_chunks) + batch_size - 1) // batch_size

        # Update progress for embedding generation
        if task_id and task_id in self.active_tasks:
            self.active_tasks[task_id]["progress"] = {
                "stage": "generating_embeddings",
                "batches_processed": 0,
                "total_batches": total_batches,
                "chunks_created": 0,
                "total_chunks": len(all_chunks)
            }

        for i in range(0, len(all_chunks), batch_size):
            # Check if cancelled
            if task_id and task_id in self.active_tasks:
                if self.active_tasks[task_id].get("cancelled"):
                    return {
                        "status": "cancelled",
                        "message": "Indexing was cancelled during embedding generation",
                        "chunks_processed": len(vectors),
                        "total_chunks": len(all_chunks)
                    }
                # Update progress
                self.active_tasks[task_id]["progress"]["batches_processed"] = i // batch_size + 1
                self.active_tasks[task_id]["progress"]["chunks_created"] = len(vectors)

            batch = all_chunks[i:i + batch_size]
            texts = [chunk["text"] for chunk in batch]

            try:
                print(f"Processing batch {i//batch_size + 1}/{total_batches}: {len(texts)} texts")
                embeddings = await self.embeddings_client.get_embeddings(texts)
                print(f"Generated {len(embeddings)} embeddings")

                for chunk, embedding in zip(batch, embeddings):
                    vectors.append({
                        "text": chunk["text"],
                        "file": chunk["file"],
                        "chunk_index": chunk["chunk_index"],
                        "embedding": embedding
                    })
            except Exception as e:
                print(f"Error generating embeddings for batch {i//batch_size}: {e}")
                import traceback
                traceback.print_exc()

        print(f"Writing {len(vectors)} vectors to {vector_file}")
        with open(vector_file, 'w', encoding='utf-8') as f:
            for vector in vectors:
                f.write(json.dumps(vector, ensure_ascii=False) + '\n')
        print(f"Successfully wrote vectors to {vector_file}")

        # Update config with indexed folder information
        if self.config_manager:
            # Count unique files that were actually processed
            unique_files = set()
            for vector in vectors:
                if "file" in vector:
                    unique_files.add(vector["file"])

            metadata = {
                "files_indexed": len(unique_files),
                "chunks_created": len(vectors),
                "file_count": len(unique_files),  # New format
                "chunk_count": len(vectors),     # New format
                "document_count": len(vectors),  # Keep for backward compatibility
                "chunk_size": self.chunk_size,
                "chunk_overlap": self.chunk_overlap
            }
            self.config_manager.add_indexed_folder(folder_path, metadata)

        # Clean up task
        if task_id and task_id in self.active_tasks:
            del self.active_tasks[task_id]

        return {
            "status": "success",
            "message": "Indexing completed successfully",
            "files_indexed": len(files_to_index),
            "chunks_created": len(vectors),
            "vector_file": vector_file
        }