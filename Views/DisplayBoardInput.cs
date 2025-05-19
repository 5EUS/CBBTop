using System.Globalization;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CBBTop.Views;
public partial class DisplayBoardInput : Window
{

    private EngineManager _engineManager;
    private string _command = "/displayboard ";
    public DisplayBoardInput(EngineManager engineManager)
    {
        _engineManager = engineManager;
        InitializeComponent();

    }

    private void ViewBitboard_Click(object? sender, RoutedEventArgs e)
    {
        var command = BitboardInput.Text?.Trim();
        if (!string.IsNullOrEmpty(command))
        {
            _engineManager.Send(_command + command);
            BitboardInput.Text = string.Empty;
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
