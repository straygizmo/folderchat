import numpy as np
from pathlib import Path
import gguf
from typing import Dict, Any, Optional


class GGUFEmbeddingModel:
    """GGUF Embedding Model Loader"""

    def __init__(self, model_path: str):
        self.model_path = Path(model_path)
        if not self.model_path.exists():
            raise FileNotFoundError(f"Model file not found: {self.model_path}")

        print(f"Loading GGUF model from: {self.model_path}")
        self.reader = gguf.GGUFReader(str(self.model_path))
        self.metadata = self._load_metadata()
        self.tensors = self._load_tensor_info()

        print(f"Model architecture: {self.metadata.get('architecture', 'unknown')}")
        print(f"Embedding dimension: {self.metadata.get('embedding_length', 'unknown')}")

    def _load_metadata(self) -> Dict[str, Any]:
        """Load model metadata"""
        metadata = {}
        for field in self.reader.fields.values():
            key = field.name.replace("general.", "").replace("gemma-embedding.", "")
            try:
                value = field.parts[field.data[0]]
                # Convert numpy arrays to Python types
                if isinstance(value, np.ndarray):
                    if value.size == 1:
                        value = value.item()
                    elif value.dtype == np.uint8:
                        # Byte array - decode to string
                        value = bytes(value).decode('utf-8', errors='ignore')
                    elif value.dtype.kind == 'U' or value.dtype.kind == 'S':
                        # String array
                        value = str(value)
                metadata[key] = value
            except (IndexError, KeyError, UnicodeDecodeError):
                pass
        return metadata

    def _load_tensor_info(self) -> Dict[str, tuple]:
        """Load tensor information (shapes only, not data)"""
        tensors = {}
        for tensor in self.reader.tensors:
            tensors[tensor.name] = tuple(tensor.shape)
        return tensors

    def get_info(self) -> Dict[str, Any]:
        """Get model information"""
        return {
            "architecture": self.metadata.get("architecture"),
            "embedding_length": self.metadata.get("embedding_length"),
            "context_length": self.metadata.get("context_length"),
            "block_count": self.metadata.get("block_count"),
            "num_tensors": len(self.tensors),
            "tensor_names": list(self.tensors.keys())[:10]  # First 10 tensor names
        }


def load_model(model_path: Optional[str] = None) -> GGUFEmbeddingModel:
    """Load GGUF embedding model

    Args:
        model_path: Path to GGUF model file. If None, uses default path.

    Returns:
        GGUFEmbeddingModel instance
    """
    if model_path is None:
        # Default path relative to project root
        model_path = "models/embedding/unsloth/embeddinggemma-300M-Q8_0.gguf"

    path = Path(model_path)
    if not path.is_absolute():
        # Try to find from project root
        project_root = Path(__file__).parent.parent
        path = project_root / path

    return GGUFEmbeddingModel(str(path))


def get_embedding(model: GGUFEmbeddingModel, text: str) -> np.ndarray:
    """Get embedding vector for text

    Note: This is a placeholder implementation. Full embedding generation
    requires implementing tokenization and forward pass through the model.

    Args:
        model: GGUFEmbeddingModel instance
        text: Input text to embed

    Returns:
        Embedding vector as numpy array
    """
    # Full implementation would require:
    # 1. Tokenization using the model's tokenizer
    # 2. Forward pass through transformer layers
    # 3. Pooling strategy (mean pooling for this model)

    embedding_dim = model.metadata.get("embedding_length", 768)

    # Return a dummy embedding for demonstration
    # In production, this should be replaced with actual model inference
    return np.random.randn(embedding_dim)


def cosine_similarity(vec1: np.ndarray, vec2: np.ndarray) -> float:
    """Calculate cosine similarity between two vectors

    Args:
        vec1: First vector
        vec2: Second vector

    Returns:
        Cosine similarity score between -1 and 1
    """
    return np.dot(vec1, vec2) / (np.linalg.norm(vec1) * np.linalg.norm(vec2))
