#define DEBUG
using System;
using System.Diagnostics;
using System.IO;
using Avalonia.Threading;
using CBBTop.Views;

namespace CBBTop;
public static class LogManager
{
    #if DEBUG
    public static readonly string ProjectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
    #else
    public static readonly string ProjectRoot = Path.GetFullPath(AppContext.BaseDirectory);
    #endif
    private static readonly string path = Path.Combine(ProjectRoot, "log_viewer.py");
    private static Process? _process;
    public static Process? Process => _process; 

    public static void RunPythonScript(string scriptPath, ConsoleWindow window)
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

        _process = new Process { StartInfo = startInfo };
        _process.OutputDataReceived += (s, e) => { if (e.Data != null) Dispatcher.UIThread.Post(() => window.Command(e.Data)); };
        _process.ErrorDataReceived += (s, e) => { if (e.Data != null) Dispatcher.UIThread.Post(() => window.Command(e.Data)); };

        _process.Start();
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
    }
}