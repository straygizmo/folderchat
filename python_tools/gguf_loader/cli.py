"""Command-line interface for GGUF Embedding Model Loader"""

from .loader import load_model, get_embedding, cosine_similarity


def main():
    """Main CLI entry point"""
    print("Loading GGUF embedding model...")
    model = load_model()

    # Show model information
    print("\n=== Model Information ===")
    info = model.get_info()
    for key, value in info.items():
        if key == "tensor_names":
            print(f"{key}: {value} ...")
        else:
            print(f"{key}: {value}")

    # Example usage
    texts = [
        "The cat sits on the mat",
        "A feline rests on a rug",
        "Python is a programming language"
    ]

    print("\n=== Generating Embeddings ===")
    embeddings = [get_embedding(model, text) for text in texts]

    print(f"\nEmbedding dimension: {len(embeddings[0])}")

    # Calculate similarities
    print("\n=== Cosine Similarities ===")
    print(f"Text 1 vs Text 2: {cosine_similarity(embeddings[0], embeddings[1]):.4f}")
    print(f"Text 1 vs Text 3: {cosine_similarity(embeddings[0], embeddings[2]):.4f}")
    print(f"Text 2 vs Text 3: {cosine_similarity(embeddings[1], embeddings[2]):.4f}")

    print("\n=== Note ===")
    print("This is a basic GGUF reader that loads the model structure.")
    print("Full embedding generation requires implementing:")
    print("1. Tokenization using the model's tokenizer")
    print("2. Forward pass through the transformer layers")
    print("3. Pooling strategy (mean pooling for this model)")
    print("\nFor production use, consider using llama.cpp with gemma-embedding support,")
    print("or the original model with transformers library.")


if __name__ == "__main__":
    main()
