#!/usr/bin/env python3
"""
Test script for embedding configuration
"""
import sys
import json
import asyncio
import httpx


async def test_embedding(config_path: str):
    """Test embedding API connection with given configuration"""
    try:
        # Load configuration
        with open(config_path, 'r', encoding='utf-8') as f:
            config = json.load(f)

        embedding_url = config.get('embedding_url', '')
        embedding_model = config.get('embedding_model', '')
        api_key = config.get('api_key', '')

        if not embedding_url:
            return {
                'success': False,
                'error': 'Embedding URL is required'
            }

        if not embedding_model:
            return {
                'success': False,
                'error': 'Embedding Model is required'
            }

        # Test text
        test_text = "This is a test to verify the embedding configuration."

        # Prepare endpoint URL
        base_url = embedding_url.rstrip('/')
        if base_url.endswith('/v1'):
            endpoint = f"{base_url}/embeddings"
        else:
            endpoint = f"{base_url}/v1/embeddings"

        print(f"Testing embedding connection...", flush=True)
        print(f"Endpoint: {endpoint}", flush=True)
        print(f"Model: {embedding_model}", flush=True)

        # Make request
        timeout = httpx.Timeout(30.0, connect=10.0)
        headers = {}
        if api_key:
            headers['Authorization'] = f'Bearer {api_key}'

        async with httpx.AsyncClient(timeout=timeout) as client:
            response = await client.post(
                endpoint,
                json={
                    "model": embedding_model,
                    "input": test_text
                },
                headers=headers
            )

            print(f"Response status: {response.status_code}", flush=True)

            if response.status_code != 200:
                error_text = response.text
                if "model_not_found" in error_text.lower():
                    return {
                        'success': False,
                        'error': f"Model '{embedding_model}' not found. Please ensure the model is loaded in the embedding service."
                    }
                return {
                    'success': False,
                    'error': f"HTTP {response.status_code}: {error_text}"
                }

            data = response.json()

            # Validate response format
            if "data" not in data or len(data["data"]) == 0:
                return {
                    'success': False,
                    'error': 'Invalid response format: missing or empty data field'
                }

            if "embedding" not in data["data"][0]:
                return {
                    'success': False,
                    'error': 'Invalid response format: missing embedding field'
                }

            embedding = data["data"][0]["embedding"]
            dimension = len(embedding)

            # Get first 5 values for display
            sample_values = embedding[:5] if len(embedding) >= 5 else embedding

            return {
                'success': True,
                'dimension': dimension,
                'sample_values': sample_values,
                'model': embedding_model,
                'url': embedding_url
            }

    except httpx.ConnectError as e:
        return {
            'success': False,
            'error': f'Connection failed: Cannot connect to {embedding_url}. Please ensure the service is running.'
        }
    except httpx.TimeoutException:
        return {
            'success': False,
            'error': 'Request timeout. The embedding service might be overloaded or not responding.'
        }
    except FileNotFoundError:
        return {
            'success': False,
            'error': f'Configuration file not found: {config_path}'
        }
    except json.JSONDecodeError as e:
        return {
            'success': False,
            'error': f'Invalid JSON in configuration file: {str(e)}'
        }
    except Exception as e:
        return {
            'success': False,
            'error': f'Unexpected error: {str(e)}'
        }


async def main():
    if len(sys.argv) < 2:
        result = {
            'success': False,
            'error': 'Usage: test_embedding.py <config_file>'
        }
        print(json.dumps(result))
        sys.exit(1)

    config_path = sys.argv[1]
    result = await test_embedding(config_path)

    # Output result as JSON
    print(json.dumps(result), flush=True)

    # Exit with appropriate code
    sys.exit(0 if result['success'] else 1)


if __name__ == "__main__":
    asyncio.run(main())
