import os
import json
from typing import List, Dict, Any, Tuple
from embeddings import EmbeddingsClient

class VectorRetriever:
    def __init__(self, config_manager=None):
        self.config_manager = config_manager
        self.embeddings_client = EmbeddingsClient(config_manager=config_manager)

        # Load settings from config if available
        if config_manager:
            self.top_k = config_manager.get("rag_settings.top_k", 3)
            self.similarity_threshold = config_manager.get("rag_settings.similarity_threshold", 0.15)
            self.max_context_length = config_manager.get("rag_settings.max_context_length", 3000)
        else:
            self.top_k = 3  # Reduce number of chunks to avoid context overflow
            self.similarity_threshold = 0.15  # Slightly higher threshold for better relevance
            self.max_context_length = 3000  # Maximum characters for context

    def update_config(self, max_context_length: int = None):
        """Update retriever configuration"""
        if max_context_length is not None:
            self.max_context_length = max_context_length

    def load_vectors(self, folder_path: str) -> List[Dict[str, Any]]:
        vector_file = os.path.join(folder_path, ".vectors.jsonl")

        if not os.path.exists(vector_file):
            return []

        vectors = []
        try:
            with open(vector_file, 'r', encoding='utf-8') as f:
                for line in f:
                    if line.strip():
                        vectors.append(json.loads(line))
        except Exception as e:
            print(f"Error loading vectors: {e}")
            return []

        return vectors

    def find_similar_chunks(
        self,
        query_embedding: List[float],
        vectors: List[Dict[str, Any]],
        top_k: int = None
    ) -> List[Tuple[Dict[str, Any], float]]:
        if top_k is None:
            top_k = self.top_k

        similarities = []
        for vector in vectors:
            similarity = self.embeddings_client.cosine_similarity(
                query_embedding,
                vector["embedding"]
            )
            if similarity >= self.similarity_threshold:
                similarities.append((vector, similarity))

        similarities.sort(key=lambda x: x[1], reverse=True)

        return similarities[:top_k]

    async def retrieve(self, folder_path: str, query: str) -> str:
        print(f"\n=== RAG RETRIEVAL DEBUG ===")
        print(f"Query: {query}")

        vectors = self.load_vectors(folder_path)
        print(f"Loaded {len(vectors)} vectors from {folder_path}")

        if not vectors:
            print("No vectors found, returning empty context")
            return ""

        try:
            query_embedding = await self.embeddings_client.get_embedding(query)
            print(f"Generated query embedding (dim: {len(query_embedding)})")
        except Exception as e:
            print(f"Error generating query embedding: {e}")
            return ""

        similar_chunks = self.find_similar_chunks(query_embedding, vectors)
        print(f"Found {len(similar_chunks)} chunks above threshold {self.similarity_threshold}")

        if not similar_chunks:
            print("No similar chunks found above threshold")
            # Show top results even if below threshold for debugging
            all_similarities = []
            for vector in vectors[:10]:  # Check first 10 for debugging
                similarity = self.embeddings_client.cosine_similarity(
                    query_embedding, vector["embedding"]
                )
                all_similarities.append((vector["text"][:100], similarity))
            all_similarities.sort(key=lambda x: x[1], reverse=True)
            print("\nTop similarities (even below threshold):")
            for text, sim in all_similarities[:3]:
                print(f"  Score: {sim:.4f} | Text: {text}...")
            return ""

        print("\nSelected chunks for context:")
        context_parts = []
        seen_texts = set()
        total_length = 0

        for i, (chunk, similarity) in enumerate(similar_chunks):
            chunk_text = chunk["text"]
            if chunk_text not in seen_texts:
                file_info = f"[File: {chunk['file']}]"
                chunk_with_info = f"{file_info}\n{chunk_text}"

                print(f"\nChunk {i+1}:")
                print(f"  Similarity: {similarity:.4f}")
                print(f"  File: {chunk['file']}")
                print(f"  Text preview: {chunk_text[:200]}...")

                # For the first chunk, truncate if needed to ensure we return something
                if i == 0 and len(chunk_with_info) > self.max_context_length:
                    # Truncate the first chunk to fit within max_context_length
                    available_space = self.max_context_length - len(file_info) - 1  # -1 for newline
                    truncated_text = chunk_text[:available_space]
                    chunk_with_info = f"{file_info}\n{truncated_text}"
                    print(f"  Truncated first chunk to {len(chunk_with_info)} characters")
                    seen_texts.add(chunk_text)
                    context_parts.append(chunk_with_info)
                    break  # No room for more chunks

                # Check if adding this chunk would exceed max length
                if total_length + len(chunk_with_info) > self.max_context_length:
                    print(f"  Skipped (would exceed max context length of {self.max_context_length})")
                    break

                seen_texts.add(chunk_text)
                context_parts.append(chunk_with_info)
                total_length += len(chunk_with_info) + 7  # Account for separator

        final_context = "\n\n---\n\n".join(context_parts)
        print(f"\nFinal context length: {len(final_context)} characters")
        print("=== END RAG DEBUG ===\n")
        return final_context

    async def retrieve_from_multiple(self, folder_paths: List[str], query: str) -> str:
        """Retrieve context from multiple folders and merge results"""
        print(f"\n=== MULTI-FOLDER RAG RETRIEVAL DEBUG ===")
        print(f"Query: {query}")
        print(f"Folders: {folder_paths}")

        # Load vectors from all folders
        all_vectors = []
        for folder_path in folder_paths:
            vectors = self.load_vectors(folder_path)
            print(f"Loaded {len(vectors)} vectors from {folder_path}")

            # Add folder source to each vector
            for vector in vectors:
                vector["source_folder"] = folder_path

            all_vectors.extend(vectors)

        print(f"Total vectors from all folders: {len(all_vectors)}")

        if not all_vectors:
            print("No vectors found in any folder")
            return ""

        try:
            query_embedding = await self.embeddings_client.get_embedding(query)
            print(f"Generated query embedding (dim: {len(query_embedding)})")
        except Exception as e:
            print(f"Error generating query embedding: {e}")
            return ""

        # Find similar chunks from all vectors
        similar_chunks = self.find_similar_chunks(query_embedding, all_vectors)
        print(f"Found {len(similar_chunks)} chunks above threshold {self.similarity_threshold}")

        if not similar_chunks:
            print("No similar chunks found above threshold")
            return ""

        print("\nSelected chunks for context:")
        context_parts = []
        seen_texts = set()
        total_length = 0

        for i, (chunk, similarity) in enumerate(similar_chunks):
            chunk_text = chunk["text"]
            if chunk_text not in seen_texts:
                # Include source folder information
                folder_name = os.path.basename(chunk.get("source_folder", "Unknown"))
                file_info = f"[Folder: {folder_name} | File: {chunk['file']}]"
                chunk_with_info = f"{file_info}\n{chunk_text}"

                print(f"\nChunk {i+1}:")
                print(f"  Similarity: {similarity:.4f}")
                print(f"  Source: {chunk.get('source_folder', 'Unknown')}")
                print(f"  File: {chunk['file']}")
                print(f"  Text preview: {chunk_text[:200]}...")

                # Check length constraints
                if i == 0 and len(chunk_with_info) > self.max_context_length:
                    available_space = self.max_context_length - len(file_info) - 1
                    truncated_text = chunk_text[:available_space]
                    chunk_with_info = f"{file_info}\n{truncated_text}"
                    print(f"  Truncated first chunk to {len(chunk_with_info)} characters")
                    seen_texts.add(chunk_text)
                    context_parts.append(chunk_with_info)
                    break

                if total_length + len(chunk_with_info) > self.max_context_length:
                    print(f"  Skipped (would exceed max context length of {self.max_context_length})")
                    break

                seen_texts.add(chunk_text)
                context_parts.append(chunk_with_info)
                total_length += len(chunk_with_info) + 7  # Account for separator

        final_context = "\n\n---\n\n".join(context_parts)
        print(f"\nFinal context length: {len(final_context)} characters")
        print("=== END MULTI-FOLDER RAG DEBUG ===\n")
        return final_context

    async def search(
        self,
        folder_path: str,
        query: str,
        top_k: int = None
    ) -> List[Dict[str, Any]]:
        vectors = self.load_vectors(folder_path)

        if not vectors:
            return []

        try:
            query_embedding = await self.embeddings_client.get_embedding(query)
        except Exception as e:
            print(f"Error generating query embedding: {e}")
            return []

        similar_chunks = self.find_similar_chunks(query_embedding, vectors, top_k)

        results = []
        for chunk, similarity in similar_chunks:
            results.append({
                "text": chunk["text"],
                "file": chunk["file"],
                "chunk_index": chunk["chunk_index"],
                "similarity": similarity
            })

        return results