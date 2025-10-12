"""GGUF Embedding Model Loader for text vectorization"""

from .loader import GGUFEmbeddingModel, load_model, get_embedding, cosine_similarity

__version__ = "0.1.0"
__all__ = ["GGUFEmbeddingModel", "load_model", "get_embedding", "cosine_similarity"]
