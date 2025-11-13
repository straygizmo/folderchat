using System.Diagnostics;
using System.Reflection;

namespace folderchat.Services
{
    public static class PythonPathHelper
    {
        private static readonly string _pythonToolsDir;
        private static readonly string _pythonExecutable;
        private static readonly bool _isDebugMode;

        static PythonPathHelper()
        {
            _isDebugMode = IsDebugEnvironment();
            _pythonToolsDir = GetPythonToolsDirectory();
            _pythonExecutable = GetPythonExecutablePath();

            ValidatePythonEnvironment();
        }

        public static string PythonToolsDirectory => _pythonToolsDir;
        public static string PythonExecutable => _pythonExecutable;
        public static bool IsDebugMode => _isDebugMode;

        public static string GetScriptPath(string scriptName)
        {
            var scriptPath = Path.Combine(_pythonToolsDir, scriptName);
            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException($"Python script not found: {scriptPath}");
            }
            return scriptPath;
        }

        // New: helpers to validate venv portability on target machines
        private static string? GetVenvHomePath()
        {
            try
            {
                var cfg = Path.Combine(_pythonToolsDir, ".venv", "pyvenv.cfg");
                if (!File.Exists(cfg)) return null;
                foreach (var line in File.ReadAllLines(cfg))
                {
                    var trimmed = line.Trim();
                    if (trimmed.StartsWith("home", StringComparison.OrdinalIgnoreCase))
                    {
                        var idx = trimmed.IndexOf('=');
                        if (idx >= 0)
                        {
                            return trimmed[(idx + 1)..].Trim();
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        private static bool IsVenvUsable(string venvPython)
        {
            try
            {
                // If pyvenv.cfg points to a base interpreter that doesn't exist (e.g. uv-managed path on another PC),
                // this venv won't be portable.
                var home = GetVenvHomePath();
                if (!string.IsNullOrEmpty(home) && !Directory.Exists(home))
                {
                    return false;
                }

                var psi = new ProcessStartInfo
                {
                    FileName = venvPython,
                    Arguments = "-c \"import sys; print('OK')\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = _pythonToolsDir
                };
                using var p = new Process { StartInfo = psi };
                p.Start();
                var output = p.StandardOutput.ReadToEnd();
                p.WaitForExit(3000);
                return p.ExitCode == 0 && output.Contains("OK");
            }
            catch
            {
                return false;
            }
        }

        private static bool IsCommandUsable(string cmd, string args = "-c \"import sys; print('OK')\"")
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = cmd,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = _pythonToolsDir
                };
                using var p = new Process { StartInfo = psi };
                p.Start();
                var output = p.StandardOutput.ReadToEnd();
                p.WaitForExit(3000);
                return p.ExitCode == 0 && output.Contains("OK");
            }
            catch
            {
                return false;
            }
        }

        private static bool IsDebugEnvironment()
        {
            // Check multiple indicators for debug/development environment
            #if DEBUG
                return true;
            #else
                // Additional runtime checks
                return Debugger.IsAttached;
            #endif
        }

        private static string GetPythonToolsDirectory()
        {
            // Always check for python_tools in the same directory as the executable first.
            var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                        ?? AppDomain.CurrentDomain.BaseDirectory;
            var localPythonToolsDir = Path.Combine(exeDir, "python_tools");

            if (Directory.Exists(localPythonToolsDir))
            {
                return localPythonToolsDir;
            }

            // If not found, and we are in a debug environment, try to find it in the solution root.
            if (_isDebugMode)
            {
                // Development mode: Navigate up to solution directory
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var solutionDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", ".."));
                var solutionPythonToolsDir = Path.Combine(solutionDir, "python_tools");
                if (Directory.Exists(solutionPythonToolsDir))
                {
                    return solutionPythonToolsDir;
                }
            }

            // If still not found, return the path next to the executable, 
            // so the subsequent error message is clear.
            return localPythonToolsDir;
        }

        private static string GetPythonExecutablePath()
        {
            // Check for virtual environment first
            var venvPython = Path.Combine(_pythonToolsDir, ".venv", "Scripts", "python.exe");
            if (File.Exists(venvPython) && IsVenvUsable(venvPython))
            {
                return venvPython;
            }

            // Fallback to python.exe in python_tools directory
            var localPython = Path.Combine(_pythonToolsDir, "python.exe");
            if (File.Exists(localPython))
            {
                return localPython;
            }

            // Try system Python/py on PATH as a last resort
            if (IsCommandUsable("py"))
            {
                return "py";
            }
            if (IsCommandUsable("python"))
            {
                return "python";
            }

            // If no local Python found in release mode, throw informative error
            if (!_isDebugMode)
            {
                throw new FileNotFoundException(
                    $"Python executable not found. Expected one of:\n" +
                    $"1) Virtual environment: {venvPython} (must be portable; pyvenv.cfg 'home' must exist on target PC)\n" +
                    $"2) Local Python: {localPython}\n" +
                    $"If a .venv was created using uv or a per-user Python, it may not be portable. Consider bundling a relocatable Python at python_tools\\python.exe.");
            }

            // In debug mode, provide more detailed error
            throw new FileNotFoundException(
                $"Python executable not found at expected locations:\n" +
                $"1. Virtual environment: {venvPython} (ignored if its base interpreter from pyvenv.cfg is missing)\n" +
                $"2. Local Python: {localPython}\n" +
                $"Please bundle a relocatable Python at python_tools\\python.exe, or create a portable environment not tied to uv/AppData.");
        }

        private static void ValidatePythonEnvironment()
        {
            if (!Directory.Exists(_pythonToolsDir))
            {
                var mode = _isDebugMode ? "Debug" : "Release";
                throw new DirectoryNotFoundException(
                    $"Python tools directory not found at: {_pythonToolsDir}\n" +
                    $"Running in {mode} mode. " +
                    (_isDebugMode
                        ? "Please ensure python_tools folder exists in solution root."
                        : "Please ensure python_tools folder is deployed alongside the executable."));
            }

            if (Path.IsPathRooted(_pythonExecutable))
            {
                if (!File.Exists(_pythonExecutable))
                {
                    throw new FileNotFoundException($"Python executable not found at: {_pythonExecutable}");
                }
            }
            else
            {
                // _pythonExecutable is a command like 'py' or 'python'; verify it is usable
                if (!IsCommandUsable(_pythonExecutable))
                {
                    throw new FileNotFoundException($"Python command '{_pythonExecutable}' not found in PATH or not usable");
                }
            }
        }

        public static string GetEnvironmentInfo()
        {
            var venvHome = GetVenvHomePath();
            return $"Python Environment Info:\n" +
                   $"Mode: {(_isDebugMode ? "Debug/Development" : "Release/Production")}\n" +
                   $"Python Tools Dir: {_pythonToolsDir}\n" +
                   $"Python Executable: {_pythonExecutable}\n" +
                   $"Tools Dir Exists: {Directory.Exists(_pythonToolsDir)}\n" +
                   $"Python Exists: {File.Exists(_pythonExecutable)}\n" +
                   $"Venv Home: {(venvHome ?? "(none)")}\n" +
                   $"Venv Home Exists: {(venvHome != null && Directory.Exists(venvHome))}";
        }
    }
}
