using System.Diagnostics;

namespace folderchat.Services
{
    public class PythonInteropService
    {
        private readonly string _pythonExecutable;
        private readonly string _scriptPath;

        public PythonInteropService()
        {
            _pythonExecutable = PythonPathHelper.PythonExecutable;
            _scriptPath = PythonPathHelper.GetScriptPath("convert_to_markdown.py");
        }

        public async Task<string> ConvertToMarkdownAsync(string inputFilePath)
        {
            if (!File.Exists(inputFilePath))
            {
                throw new FileNotFoundException($"Input file not found: {inputFilePath}");
            }

            var outputFilePath = Path.ChangeExtension(inputFilePath, ".md");

            var processStartInfo = new ProcessStartInfo
            {
                FileName = _pythonExecutable,
                Arguments = $"\"{_scriptPath}\" \"{inputFilePath}\" \"{outputFilePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processStartInfo };

            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to start Python process. Is Python installed and in PATH? Error: {ex.Message}");
            }

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                throw new Exception($"Python conversion failed (exit code {process.ExitCode}): {error}\nOutput: {output}\nScript: {_scriptPath}\nCommand: python \"{_scriptPath}\" \"{inputFilePath}\" \"{outputFilePath}\"");
            }

            if (!File.Exists(outputFilePath))
            {
                throw new Exception($"Python script completed but output file was not created: {outputFilePath}");
            }

            return outputFilePath;
        }
    }
}