using System;
using Avalonia.Input;
using Avalonia.Interactivity;
using CBBTop.ViewModels;
using MsBox.Avalonia;

namespace CBBTop.Views;

public partial class MainWindow : CBBWindow
{

    private MainWindowViewModel ViewModel => (DataContext as MainWindowViewModel)!;

    public MainWindow()
    {
        InitializeComponent();

        var binder = new Keybinds(KeyBindings);
        binder.AddKeyBinding(Key.F2, () => CloseEngine_Click(this, null));
        binder.AddKeyBinding(Key.Q, () => Exit_Click(this, null), KeyModifiers.Control);
        binder.AddKeyBinding(Key.D, () => DisplayBoard_Click(this, null), KeyModifiers.Control);
        binder.AddKeyBinding(Key.A, () => GetAttack_Click(this, null), KeyModifiers.Control);
        binder.AddKeyBinding(Key.M, () => GetMagicIndex_Click(this, null), KeyModifiers.Control);
        binder.AddKeyBinding(Key.P, () => GetPermIndex_Click(this, null), KeyModifiers.Control);
        binder.AddKeyBinding(Key.H, () => Help_Click(this, null), KeyModifiers.Control);
        binder.AddKeyBinding(Key.T, () => RunSlidingTest_Click(this, null), KeyModifiers.Control);
        binder.AddKeyBinding(Key.S, () => GetSlidingAttack_Click(this, null), KeyModifiers.Control);
        binder.AddKeyBinding(Key.T, () => ToggleTimer_Click(this, null), KeyModifiers.Shift);
        binder.AddKeyBinding(Key.T, () => UnitTests_Click(this, null));
        binder.AddKeyBinding(Key.C, () => Console_Click(this, null), KeyModifiers.Control);
        binder.AddKeyBinding(Key.L, () => EngineLog_Click(this, null), KeyModifiers.Control);
        binder.AddKeyBinding(Key.F1, () => OpenEngine_Click(this, null));
 
    }

    private async void OpenEngine_Click(object? sender, RoutedEventArgs? e)
    {
        ViewModel.ConsoleWindow?.Show();
        var path = await OpenWindowGetFilePath("Open Engine Executable");
        ViewModel.EngineManager.Start(path);
    }

    private void CloseEngine_Click(object? sender, RoutedEventArgs? e)
    {
        ViewModel.EngineManager.Stop();
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
        if (ViewModel.DisplayBoardInput == null || ViewModel.DisplayBoardInput.IsVisible == false)
        {
            ViewModel.DisplayBoardInput = new DisplayBoardInput();
        }
        ViewModel.DisplayBoardInput.Show();
    }

    private void GetAttack_Click(object? sender, RoutedEventArgs? e)
    {
        var display = new DisplayAttackInput(ViewModel.EngineManager, "/getattack ", () =>
        {
        }, new AttackViewModel());
        display.Show();
    }

    private void GetMagicIndex_Click(object? sender, RoutedEventArgs? e)
    {
        Console.WriteLine("Get Magic Index clicked");
        ViewModel.ConsoleWindow?.Log("Get Magic Index clicked");
    }

    private void GetPermIndex_Click(object? sender, RoutedEventArgs? e)
    {
        Console.WriteLine("Get Perm Index clicked");
        ViewModel.ConsoleWindow?.Log("Get Perm Index clicked");
    }

    private void RunSlidingTest_Click(object? sender, RoutedEventArgs? e)
    {
        Console.WriteLine("Run Sliding Test clicked");
        ViewModel.ConsoleWindow?.Log("Run Sliding Test clicked");
    }

    private void ToggleTimer_Click(object? sender, RoutedEventArgs? e)
    {
        Console.WriteLine("Toggle Timer clicked");
        ViewModel.ConsoleWindow?.Log("Toggle Timer clicked");
    }

    private void EngineSettings_Click(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Engine Settings clicked");
        ViewModel.ConsoleWindow?.Log("Engine Settings clicked");
    }

    private void BoardSettings_Click(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Board Settings clicked");
        ViewModel.ConsoleWindow?.Log("Board Settings clicked");
    }

    private void ThemeSettings_Click(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Theme Settings clicked");
        ViewModel.ConsoleWindow?.Log("Theme Settings clicked");
    }

    private async void EngineLog_Click(object? sender, RoutedEventArgs? e)
    {
        var path = await OpenWindowGetFilePath("Open Engine Log File");
        if (!string.IsNullOrEmpty(path))
        {
            ViewModel.ConsoleWindow?.Log($"Engine log file: {path}");
            var window = new LogViewerWindow(path);
            window.LoadLogFile();
            window.Show();
        }
        else
        {
            ViewModel.ConsoleWindow?.Log("No file selected.");
        }
    }

    private void Console_Click(object? sender, RoutedEventArgs? e)
    {
        if (ViewModel.ConsoleWindow == null || ViewModel.ConsoleWindow.IsVisible == false)
        {
            ViewModel.ConsoleWindow = new ConsoleWindow(ViewModel.EngineManager.Send);
        }
        ViewModel.ConsoleWindow.Show();
    }

    private void GetSlidingAttack_Click(object? sender, RoutedEventArgs? e)
    {
        Console.WriteLine("Get Sliding Attack clicked");
        ViewModel.ConsoleWindow?.Log("Get Sliding Attack clicked");
    }

    private void Help_Click(object? sender, RoutedEventArgs? e)
    {
        Console.WriteLine("Help clicked");
        ViewModel.ConsoleWindow?.Log("Help clicked");
    }

    private void UnitTests_Click(object? sender, RoutedEventArgs? e)
    {
        Console.WriteLine("Unit Tests clicked");
        ViewModel.ConsoleWindow?.Log("Unit Tests clicked");
    }

}
