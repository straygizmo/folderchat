from pathlib import Path
from typing import Optional
from markitdown import MarkItDown
import os

class DocumentConverter:
    def __init__(self):
        self.markitdown = MarkItDown()
    
    def convert_to_markdown(self, file_path: Path) -> Optional[Path]:
        print(f"[DEBUG] converter.py: Checking {file_path}", flush=True)
        try:
            md_path = file_path.with_suffix('.md')

            if md_path.exists():
                original_mtime = os.path.getmtime(file_path)
                md_mtime = os.path.getmtime(md_path)
                print(f"[DEBUG] converter.py: original_mtime={original_mtime}, md_mtime={md_mtime}", flush=True)
                if original_mtime <= md_mtime:
                    print(f"[INFO] Skipping conversion for {file_path}, .md file is up to date.", flush=True)
                    return md_path

            print(f"[DEBUG] converter.py: Converting {file_path}", flush=True)
            result = self.markitdown.convert(str(file_path))
            
            with open(md_path, 'w', encoding='utf-8') as f:
                f.write(result.text_content)
            
            return md_path
        except Exception as e:
            print(f"Error converting file: {e}", flush=True)
            return None