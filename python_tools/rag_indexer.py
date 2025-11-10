import sys
import json
import asyncio
from pathlib import Path
from indexer import DocumentIndexer

class SimpleConfigManager:
    def __init__(self, config_dict):
        self.config = config_dict
    
    def get(self, key, default=None):
        keys = key.split('.')
        value = self.config
        for k in keys:
            if isinstance(value, dict) and k in value:
                value = value[k]
            else:
                return default
        return value
    
    def add_indexed_folder(self, folder_path, metadata):
        pass

async def main():
    if len(sys.argv) < 2:
        print("Usage: rag_indexer.py <config_file_path>", file=sys.stderr)
        sys.exit(1)
    
    try:
        config_file = sys.argv[1]
        
        with open(config_file, 'r', encoding='utf-8') as f:
            user_config = json.load(f)
        
        folders = user_config['folders']
        embedding_url = user_config.get('embedding_url')
        embedding_model = user_config.get('embedding_model')
        context_length = user_config['context_length']
        chunk_size = user_config['chunk_size']
        chunk_overlap = user_config['chunk_overlap']
        api_key = user_config.get('api_key')
        use_native_embedding = user_config.get('use_native_embedding', False)
        gguf_model = user_config.get('gguf_model')

        chunk_size = min(chunk_size, context_length)
        chunk_overlap = min(chunk_overlap, chunk_size)
        
        config = {
            "api": {
                "embedding_base_url": embedding_url,
                "llm_base_url": embedding_url
            },
            "embedding": {
                "model": embedding_model
            },
            "rag_settings": {
                "chunk_size": chunk_size,
                "chunk_overlap": chunk_overlap
            }
        }
        
        config_manager = SimpleConfigManager(config)
        indexer = DocumentIndexer(
            config_manager=config_manager,
            use_native_embedding=use_native_embedding,
            gguf_model=gguf_model
        )
        
        print(f"Starting RAG indexing with chunk_size={chunk_size}, chunk_overlap={chunk_overlap}")
        
        for folder in folders:
            print(f"Processing folder: {folder}")
            
            result = await indexer.index_folder(folder, force_reindex=False)
            
            if result["status"] == "success":
                print(f"Success: {result['message']}")
                print(f"Files indexed: {result['files_indexed']}")
                print(f"Chunks created: {result['chunks_created']}")
                print(f"Vector file: {result['vector_file']}")
            elif result["status"] == "no_files":
                print(f"Warning: {result['message']}")
            else:
                print(f"Error: {result.get('message', 'Unknown error')}", file=sys.stderr)
                sys.exit(1)
        
        print("All folders processed successfully")
        
    except Exception as e:
        print(f"Error: {str(e)}", file=sys.stderr)
        import traceback
        traceback.print_exc(file=sys.stderr)
        sys.exit(1)

if __name__ == "__main__":
    asyncio.run(main())