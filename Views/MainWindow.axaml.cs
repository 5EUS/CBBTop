using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CBBTop.ViewModels;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;

namespace CBBTop.Views;

public partial class MainWindow : Window
{

    private ConsoleWindow? _consoleWindow;
    private EngineManager _engineManager;
    private DisplayBoardInput? _displayBoardInput;

    public MainWindow()
    {
        InitializeComponent();
        _consoleWindow = new ConsoleWindow();
        _engineManager = new EngineManager();
        _displayBoardInput = new DisplayBoardInput(_engineManager);

        _engineManager.OnEngineOutput += (output) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                _consoleWindow?.Log(output);
                DisplayBoardInput.TryExtractBitboardAndShow(output);
            });
        };

        AddKeybinding(Key.F1, () => OpenEngine_Click(this, null));
        AddKeybinding(Key.Q, () => Exit_Click(this, null), KeyModifiers.Control);
        AddKeybinding(Key.D, () => DisplayBoard_Click(this, null), KeyModifiers.Control);
        AddKeybinding(Key.A, () => GetAttack_Click(this, null), KeyModifiers.Control);
        AddKeybinding(Key.M, () => GetMagicIndex_Click(this, null), KeyModifiers.Control);
        AddKeybinding(Key.P, () => GetPermIndex_Click(this, null), KeyModifiers.Control);
        AddKeybinding(Key.H, () => Help_Click(this, null), KeyModifiers.Control);
        AddKeybinding(Key.T, () => RunSlidingTest_Click(this, null), KeyModifiers.Control);
        AddKeybinding(Key.S, () => GetSlidingAttack_Click(this, null), KeyModifiers.Control);
        AddKeybinding(Key.T, () => ToggleTimer_Click(this, null), KeyModifiers.Shift);
        AddKeybinding(Key.T, () => UnitTests_Click(this, null));
        AddKeybinding(Key.C, () => Console_Click(this, null), KeyModifiers.Control);
        AddKeybinding(Key.L, () => EngineLog_Click(this, null), KeyModifiers.Control);
 
    }

    private void AddKeybinding(Key key, Action action, KeyModifiers? modifiers = null)
    {
        KeyBindings.Add(new KeyBinding
        {
            Gesture = new KeyGesture(key, modifiers ?? KeyModifiers.None),
            Command = new RelayCommand(action)
        });
    }

    private async void OpenEngine_Click(object? sender, RoutedEventArgs? e)
    {
        
        var path = await OpenWindowGetFilePath();
        _engineManager.Start(path);

    }

    private async Task<string> OpenWindowGetFilePath()
    {
               // Request file(s) from user
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Engine Executable",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Executable")
                {
                    Patterns = ["*.exe", "*.bin", "*.out", "*"]
                }
            ]
        });

        if (files.Any())
        {
            var file = files[0];
            var path = file.Path?.LocalPath;

            if (!string.IsNullOrEmpty(path))
            {
                return path;
            }
        } 
        return string.Empty;
    }

    private void Exit_Click(object? sender, RoutedEventArgs? e)
    {
        Environment.Exit(0);
    }

    private void About_Click(object? sender, RoutedEventArgs e)
    {
        var msgBox = MessageBoxManager
            .GetMessageBoxStandard("About", "Chess engine using Avalonia UI.");
        msgBox.ShowAsPopupAsync(this);
    }

    private void DisplayBoard_Click(object? sender, RoutedEventArgs? e)
    {
        if (_displayBoardInput == null || _displayBoardInput.IsVisible == false)
        {
            _displayBoardInput = new DisplayBoardInput(_engineManager);
        }
        _displayBoardInput.Show();
    }

    private void GetAttack_Click(object? sender, RoutedEventArgs? e)
    {
        var display = new DisplayAttackInput(_engineManager, "/getattack ", () =>
        {
        }, new AttackViewModel());
        display.Show();
    }

    private void GetMagicIndex_Click(object? sender, RoutedEventArgs? e)
    {
        Console.WriteLine("Get Magic Index clicked");
        _consoleWindow?.Log("Get Magic Index clicked");
    }

    private void GetPermIndex_Click(object? sender, RoutedEventArgs? e)
    {
        Console.WriteLine("Get Perm Index clicked");
        _consoleWindow?.Log("Get Perm Index clicked");
    }

    private void RunSlidingTest_Click(object? sender, RoutedEventArgs? e)
    {
        Console.WriteLine("Run Sliding Test clicked");
        _consoleWindow?.Log("Run Sliding Test clicked");
    }

    private void ToggleTimer_Click(object? sender, RoutedEventArgs? e)
    {
        Console.WriteLine("Toggle Timer clicked");
        _consoleWindow?.Log("Toggle Timer clicked");
    }

    private void EngineSettings_Click(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Engine Settings clicked");
        _consoleWindow?.Log("Engine Settings clicked");
    }

    private void BoardSettings_Click(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Board Settings clicked");
        _consoleWindow?.Log("Board Settings clicked");
    }

    private void ThemeSettings_Click(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Theme Settings clicked");
        _consoleWindow?.Log("Theme Settings clicked");
    }

    private async void EngineLog_Click(object? sender, RoutedEventArgs? e)
    {
        var path = await OpenWindowGetFilePath();
        if (!string.IsNullOrEmpty(path))
        {
            _consoleWindow?.Log($"Engine log file: {path}");
            LogManager.RunPythonScript(path);
        }
        else
        {
            _consoleWindow?.Log("No file selected.");
        }
    }

    private void Console_Click(object? sender, RoutedEventArgs? e)
    {
        if (_consoleWindow == null || _consoleWindow.IsVisible == false)
        {
            _consoleWindow = new ConsoleWindow();
        }
        _consoleWindow.Show();
    }

    private void GetSlidingAttack_Click(object? sender, RoutedEventArgs? e)
    {
        Console.WriteLine("Get Sliding Attack clicked");
        _consoleWindow?.Log("Get Sliding Attack clicked");
    }

    private void Help_Click(object? sender, RoutedEventArgs? e)
    {
        Console.WriteLine("Help clicked");
        _consoleWindow?.Log("Help clicked");
    }

    private void UnitTests_Click(object? sender, RoutedEventArgs? e)
    {
        Console.WriteLine("Unit Tests clicked");
        _consoleWindow?.Log("Unit Tests clicked");
    }

}