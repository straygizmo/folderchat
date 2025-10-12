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

        private static bool IsDebugEnvironment()
        {
            // Check multiple indicators for debug/development environment
            #if DEBUG
                return true;
            #else
                // Additional runtime checks
                var debuggerAttached = Debugger.IsAttached;
                var currentDir = AppDomain.CurrentDomain.BaseDirectory;

                // Check if running from bin\Debug or bin\Release
                var isDevelopmentPath = currentDir.Contains(@"\bin\Debug") ||
                                       currentDir.Contains(@"\bin\Release") ||
                                       currentDir.Contains(@"/bin/Debug") ||
                                       currentDir.Contains(@"/bin/Release");

                return debuggerAttached || isDevelopmentPath;
            #endif
        }

        private static string GetPythonToolsDirectory()
        {
            if (_isDebugMode)
            {
                // Development mode: Navigate up to solution directory
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var solutionDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", ".."));
                return Path.Combine(solutionDir, "python_tools");
            }
            else
            {
                // Release mode: python_tools is in the same directory as the executable
                var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                            ?? AppDomain.CurrentDomain.BaseDirectory;
                return Path.Combine(exeDir, "python_tools");
            }
        }

        private static string GetPythonExecutablePath()
        {
            // Check for virtual environment first
            var venvPython = Path.Combine(_pythonToolsDir, ".venv", "Scripts", "python.exe");
            if (File.Exists(venvPython))
            {
                return venvPython;
            }

            // Fallback to python.exe in python_tools directory
            var localPython = Path.Combine(_pythonToolsDir, "python.exe");
            if (File.Exists(localPython))
            {
                return localPython;
            }

            // If no local Python found in release mode, throw informative error
            if (!_isDebugMode)
            {
                throw new FileNotFoundException(
                    $"Python executable not found. Please ensure python_tools folder with Python environment is in the same directory as {Path.GetFileName(Assembly.GetExecutingAssembly().Location)}. " +
                    $"Expected locations: {venvPython} or {localPython}");
            }

            // In debug mode, provide more detailed error
            throw new FileNotFoundException(
                $"Python executable not found at expected locations:\n" +
                $"1. Virtual environment: {venvPython}\n" +
                $"2. Local Python: {localPython}\n" +
                $"Please run the Python setup script or create virtual environment in python_tools directory.");
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

            if (!File.Exists(_pythonExecutable))
            {
                throw new FileNotFoundException($"Python executable not found at: {_pythonExecutable}");
            }
        }

        public static string GetEnvironmentInfo()
        {
            return $"Python Environment Info:\n" +
                   $"Mode: {(_isDebugMode ? "Debug/Development" : "Release/Production")}\n" +
                   $"Python Tools Dir: {_pythonToolsDir}\n" +
                   $"Python Executable: {_pythonExecutable}\n" +
                   $"Tools Dir Exists: {Directory.Exists(_pythonToolsDir)}\n" +
                   $"Python Exists: {File.Exists(_pythonExecutable)}";
        }
    }
}