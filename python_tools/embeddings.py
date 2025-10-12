import httpx
import os
from typing import List, Dict, Any, Optional
from dotenv import load_dotenv
import asyncio
import numpy as np

load_dotenv()

class EmbeddingsClient:
    def __init__(self, config_manager=None):
        self.config_manager = config_manager

        # Load from config manager if available, otherwise use env/defaults
        if config_manager:
            self.embedding_base_url = config_manager.get("api.embedding_base_url", "http://localhost:1234")
            self.llm_base_url = config_manager.get("api.llm_base_url", "http://localhost:1234")
            self.embedding_model = config_manager.get("embedding.model", "google/embedding-gemma-300m")
        else:
            self.embedding_base_url = os.getenv("EMBEDDING_BASE_URL", "http://localhost:1234")
            self.llm_base_url = os.getenv("LLM_BASE_URL", "http://localhost:1234")
            self.embedding_model = os.getenv("EMBEDDING_MODEL", "google/embedding-gemma-300m")

        self.timeout = httpx.Timeout(60.0, connect=10.0)

    def update_config(self, llm_base_url: str = None, embedding_base_url: str = None):
        if llm_base_url:
            self.llm_base_url = llm_base_url
        if embedding_base_url:
            self.embedding_base_url = embedding_base_url

    async def get_embeddings(self, texts: List[str]) -> List[List[float]]:
        print(f"Getting embeddings for {len(texts)} texts")
        print(f"Embedding API URL: {self.embedding_base_url}")
        print(f"Embedding model: {self.embedding_model}")

        async with httpx.AsyncClient(timeout=self.timeout) as client:
            embeddings = []

            for idx, text in enumerate(texts):
                try:
                    # Truncate text if too long (approx 4 chars per token, max 512 tokens for embedding models)
                    max_chars = 2000
                    truncated_text = text[:max_chars] if len(text) > max_chars else text
                    if len(text) > max_chars:
                        print(f"WARNING: Text truncated from {len(text)} to {len(truncated_text)} chars", flush=True)
                    
                    print(f"Processing text {idx+1}/{len(texts)}: {len(truncated_text)} chars", flush=True)
                    # Remove /v1 suffix from base_url if present to avoid duplication
                    base_url = self.embedding_base_url.rstrip('/')
                    if base_url.endswith('/v1'):
                        endpoint = f"{base_url}/embeddings"
                    else:
                        endpoint = f"{base_url}/v1/embeddings"
                    
                    response = await client.post(
                        endpoint,
                        json={
                            "model": self.embedding_model,
                            "input": truncated_text
                        }
                    )
                    print(f"Response status: {response.status_code}")

                    if response.status_code != 200:
                        error_msg = f"Embedding API error: {response.text}"
                        print(error_msg)
                        # Check if it's a model not found error
                        if "model_not_found" in response.text:
                            raise Exception(f"Embedding model '{self.embedding_model}' not loaded in LMStudio. Please load an embedding model.")
                        raise Exception(error_msg)

                    data = response.json()
                    print(f"Response keys: {list(data.keys())}", flush=True)
                    if "data" in data and len(data["data"]) > 0:
                        if "embedding" in data["data"][0]:
                            embeddings.append(data["data"][0]["embedding"])
                        else:
                            print(f"DEBUG: data[0] keys: {list(data['data'][0].keys())}", flush=True)
                            raise Exception("Invalid embedding response format: 'embedding' key not found")
                    else:
                        print(f"DEBUG: Full response: {data}", flush=True)
                        raise Exception("Invalid embedding response format: 'data' key missing or empty")
                except httpx.ConnectError:
                    raise Exception("Cannot connect to LMStudio. Please ensure LMStudio is running on port 1234.")
                except httpx.TimeoutException:
                    raise Exception("Request timeout. The embedding model might be loading or the text is too long.")

            return embeddings

    async def get_embedding(self, text: str) -> List[float]:
        embeddings = await self.get_embeddings([text])
        return embeddings[0] if embeddings else []

    async def chat_completion(
        self,
        messages: List[Dict[str, str]],
        temperature: float = 0.7,
        max_tokens: int = None
    ) -> str:
        async with httpx.AsyncClient(timeout=self.timeout) as client:
            payload = {
                "messages": messages,
                "temperature": temperature,
                "stream": False
            }

            if max_tokens:
                payload["max_tokens"] = max_tokens

            response = await client.post(
                f"{self.llm_base_url}/v1/chat/completions",
                json=payload
            )

            if response.status_code != 200:
                raise Exception(f"LLM API error: {response.text}")

            data = response.json()
            if "choices" in data and len(data["choices"]) > 0:
                return data["choices"][0]["message"]["content"]
            else:
                raise Exception("Invalid chat completion response format")

    @staticmethod
    def cosine_similarity(vec1: List[float], vec2: List[float]) -> float:
        vec1_np = np.array(vec1)
        vec2_np = np.array(vec2)

        dot_product = np.dot(vec1_np, vec2_np)
        norm1 = np.linalg.norm(vec1_np)
        norm2 = np.linalg.norm(vec2_np)

        if norm1 == 0 or norm2 == 0:
            return 0.0

        return dot_product / (norm1 * norm2)