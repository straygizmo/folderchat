#!/usr/bin/env python3
import sys
import os

def main():
    if len(sys.argv) != 3:
        print("Usage: convert_to_markdown.py <input_file> <output_file>")
        sys.exit(1)

    input_file = sys.argv[1]
    output_file = sys.argv[2]

    if not os.path.exists(input_file):
        print(f"Error: Input file '{input_file}' does not exist.")
        sys.exit(1)

    try:
        from markitdown import MarkItDown

        md = MarkItDown()
        result = md.convert(input_file)

        with open(output_file, 'w', encoding='utf-8') as f:
            f.write(result.text_content)

        print(f"Successfully converted '{input_file}' to '{output_file}'")
        sys.exit(0)

    except ImportError:
        print("Error: markitdown package is not installed.")
        print("Please install it using: pip install markitdown")
        sys.exit(1)
    except Exception as e:
        print(f"Error converting file: {str(e)}")
        sys.exit(1)

if __name__ == "__main__":
    main()