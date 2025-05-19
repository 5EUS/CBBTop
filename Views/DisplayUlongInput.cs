using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CBBTop.ViewModels;

namespace CBBTop.Views;
public partial class DisplayAttackInput : Window
{

    private EngineManager _engineManager;
    private string _command;
    private Action _action;

    public DisplayAttackInput(EngineManager engineManager, string command, Action action, ViewModelBase view)
    {
        DataContext = view;
        _engineManager = engineManager;
        _action = action;
        _command = command;
        InitializeComponent();

    }

    private void ViewBitboard_Click(object? sender, RoutedEventArgs e)
    {
        var command = BitboardInput.Text?.Trim();
        if (!string.IsNullOrEmpty(command))
        {
            Dispatcher.UIThread.Post(() =>
            {
                _engineManager.Send(_command + command);
            });
            BitboardInput.Text = string.Empty;
            _action?.Invoke();
        }
    }

    public static void TryExtractBitboardAndShow(string output)
    {
        var match = MyRegex().Match(output);
        if (match.Success)
        {
            string value = match.Groups[1].Value;

            if (ulong.TryParse(value.StartsWith("0x") ? value[2..] : value,
                            NumberStyles.HexNumber,
                            CultureInfo.InvariantCulture,
                            out ulong bitboard))
            {
                var window = new BitboardWindow(bitboard);
                window.Show();
            }
        }
    }

    [GeneratedRegex(@"bitboard:\s*(0x[0-9a-fA-F]+|\d+)")]
    private static partial Regex MyRegex();
}
