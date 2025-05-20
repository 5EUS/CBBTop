using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CBBTop.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CBBTop.Views;
public partial class LogViewerWindow : CBBWindow
{

    private string _path;
    private List<string>? _logLines;
    private Dictionary<string, Dictionary<int, Dictionary<int, AttackLogParser.AttackEntry>>>? _parsedPairs;
    private readonly int MaxLogLines = 15_000;

    public LogViewerWindow(string path)
    {
        _path = path;
        var binder = new Keybinds(KeyBindings);
        binder.AddKeyBinding(Key.F1, () => Open_Click(this, null));
        binder.AddKeyBinding(Key.F2, () => Search_Click(this, null));
        binder.AddKeyBinding(Key.F3, () => Clear_Click(this, null));
        binder.AddKeyBinding(Key.Q, () => Exit_Click(this, null), KeyModifiers.Control);
        InitializeComponent();
    }

    public void LoadLogFile()
    {
        if (string.IsNullOrEmpty(_path) || !File.Exists(_path))
        {
            LogBox.Text = "Log file not found.";
            return;
        }

        try
        {
            _logLines = File.ReadLines(_path)
                            .Reverse()
                            .Take(MaxLogLines)
                            .Reverse()
                            .ToList(); 

            LogBox.Text = string.Join(Environment.NewLine, _logLines);
            _parsedPairs = AttackLogParser.ParseLog(_logLines);
        }
        catch (Exception ex)
        {
            LogBox.Text = $"Error loading log file: {ex.Message}";
        }
    }

    public void Log(string message, string level = "INFO")
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        var logEntry = $"[{timestamp}] [{level}] {message}";
        LogBox.Text += logEntry + Environment.NewLine;
        LogBox.CaretIndex = LogBox.Text.Length;
    }

    private async void Open_Click(object? sender, RoutedEventArgs? e)
    {
        var path = await OpenWindowGetFilePath("Open Log File");
        if (!string.IsNullOrEmpty(path))
        {
            _path = path;
            LoadLogFile();
        }
    }

    private void Exit_Click(object? sender, RoutedEventArgs? e)
    {
        Close();
    }

    private void Search_Click(object? sender, RoutedEventArgs? e)
    {
        if (_logLines == null || _parsedPairs == null)
        {
            return;
        }
        var window = new FindLogWindow(_logLines, _parsedPairs, new AttackViewModel());
        window.Show();
    }

    private void Clear_Click(object? sender, RoutedEventArgs? e)
    {
        LogBox.Text = string.Empty;
    }
}
