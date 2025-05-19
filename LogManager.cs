#define DEBUG
using System;
using System.Diagnostics;
using System.IO;

namespace CBBTop;
public static class LogManager
{
    #if DEBUG
    public static readonly string ProjectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
    #else
    public static readonly string ProjectRoot = Path.GetFullPath(AppContext.BaseDirectory);
    #endif
    private static readonly string path = Path.Combine(ProjectRoot, "log_viewer.py");
    public static void RunPythonScript(string scriptPath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "python3",
            ArgumentList = { path, scriptPath },
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = false
        };

        using var process = new Process { StartInfo = startInfo };
        process.OutputDataReceived += (s, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
        process.ErrorDataReceived += (s, e) => { if (e.Data != null) Console.Error.WriteLine(e.Data); };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
    }
}