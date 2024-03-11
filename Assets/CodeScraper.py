import os
import sys

# deep copy from https://github.com/Lee-WonJun/CodePaper/ (CodePaper)

class CodeBlock:
    def __init__(self, file, lang, content):
        self.File = file
        self.Lang = lang
        self.Content = content

class ParsedArgs:
    def __init__(self):
        self.OutputPath = "output.md"
        self.FilterExts = []
        self.Help = False
        self.Files = []

def parse_args(args):
    parsed_args = ParsedArgs()
    i = 0
    while i < len(args):
        arg = args[i]
        if arg == "-o" and i + 1 < len(args):
            parsed_args.OutputPath = args[i + 1]
            i += 1
        elif arg == "-e" and i + 1 < len(args):
            parsed_args.FilterExts.append(args[i + 1])
            i += 1
        elif arg == "-h":
            parsed_args.Help = True
        else:
            if os.path.exists(arg):
                files = []
                for root, _, files_in_folder in os.walk(arg):
                    for file in files_in_folder:
                        files.append(os.path.join(root, file))
                parsed_args.Files.extend(files)
            else:
                parsed_args.Files.append(arg)
        i += 1
    return parsed_args

def get_file_content(path):
    with open(path, 'r', errors='ignore') as file:
        return file.read()

def get_lang_by_ext(ext):
    ext_map = {
        ".kt": "kotlin",
        ".java": "java",
        ".cs": "csharp",
        ".fs": "fsharp",
        ".js": "javascript",
        ".ts": "typescript",
        ".html": "html",
        ".css": "css",
        ".py": "python",
        ".sh": "bash",
        ".ps1": "powershell",
        ".bat": "batch",
        ".c": "c",
        ".cpp": "cpp",
        ".h": "cpp",
        ".hpp": "cpp",
        ".go": "go",
        ".rs": "rust",
        ".php": "php",
        ".sql": "sql",
        ".json": "json",
        ".xml": "xml",
        ".yml": "yaml",
        ".yaml": "yaml",
        ".clj": "clojure",
        ".cljs": "clojure",
        ".cljc": "clojure",
        ".edn": "clojure",
        ".lua": "lua",
        ".rb": "ruby",
        ".r": "r",
        ".scala": "scala",
        ".swift": "swift",
        ".vb": "vbnet",
        ".vbnet": "vbnet",
        ".coffee": "coffeescript",
        ".elm": "elm"
    }
    return ext_map.get(ext, "")

def get_code_block(ext, path):
    lang = get_lang_by_ext(ext)
    content = get_file_content(path)
    return CodeBlock(path, lang, content)

def generate_output(files, filter_exts, output_path):
    code_blocks = []
    for file in files:
        ext = os.path.splitext(file)[1]
        if filter_exts and ext not in filter_exts:
            continue
        code_blocks.append(get_code_block(ext, file))

    with open(output_path, 'w') as output_file:
        for block in code_blocks:
            header = f"### {block.File}\n"
            code_block = f"```{block.Lang}\n{block.Content}\n```\n"
            output_file.write(header)
            output_file.write(code_block)

def print_help():
    print("Help:\n-h: Help\n-o: Output path\n-e: File extension filter\n")

if __name__ == "__main__":
    parsed_args = parse_args(sys.argv[1:])
    if parsed_args.Help:
        print_help()
    else:
        generate_output(parsed_args.Files, parsed_args.FilterExts, parsed_args.OutputPath)
