"""
Native Embedding Server using gguf_loader
This script provides embedding generation using the local GGUF model.
"""
import sys
import json
from pathlib import Path

# gguf_loader is now in python_tools directory
from gguf_loader.loader import load_model, get_embedding


def generate_embedding(model, text):
    """Generate embedding for a single text"""
    try:
        embedding = get_embedding(model, text)
        return {
            "success": True,
            "embedding": embedding.tolist(),
            "dimension": len(embedding)
        }
    except Exception as e:
        return {
            "success": False,
            "error": str(e)
        }


def main():
    """Main entry point for native embedding server"""
    if len(sys.argv) != 2:
        print(json.dumps({
            "success": False,
            "error": "Usage: native_embedding_server.py <config_file>"
        }))
        sys.exit(1)

    config_file = sys.argv[1]

    try:
        # Load configuration
        with open(config_file, 'r', encoding='utf-8') as f:
            config = json.load(f)

        # Get model path from config or use default
        model_path = config.get('model_path')

        # Load the GGUF model
        print("Loading GGUF embedding model...")
        model = load_model(model_path)

        print(f"Model loaded: {model.metadata.get('architecture', 'unknown')}")
        print(f"Embedding dimension: {model.metadata.get('embedding_length', 'unknown')}")

        # Process text or batch of texts
        if 'text' in config:
            # Single text
            text = config['text']
            result = generate_embedding(model, text)
            print(json.dumps(result))

        elif 'texts' in config:
            # Batch of texts
            texts = config['texts']
            results = []
            for i, text in enumerate(texts):
                print(f"Processing {i+1}/{len(texts)}...")
                result = generate_embedding(model, text)
                results.append(result)

            print(json.dumps({
                "success": True,
                "results": results
            }))

        else:
            print(json.dumps({
                "success": False,
                "error": "No 'text' or 'texts' field in config"
            }))
            sys.exit(1)

    except FileNotFoundError as e:
        print(json.dumps({
            "success": False,
            "error": f"File not found: {str(e)}"
        }))
        sys.exit(1)

    except json.JSONDecodeError as e:
        print(json.dumps({
            "success": False,
            "error": f"Invalid JSON in config file: {str(e)}"
        }))
        sys.exit(1)

    except Exception as e:
        print(json.dumps({
            "success": False,
            "error": f"Error: {str(e)}"
        }))
        sys.exit(1)


if __name__ == "__main__":
    main()
