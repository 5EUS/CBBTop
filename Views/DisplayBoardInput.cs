using System.Globalization;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CBBTop.Views;
public partial class DisplayBoardInput : Window
{

    public DisplayBoardInput()
    {
        var binder = new Keybinds(KeyBindings);
        binder.AddKeyBinding(Avalonia.Input.Key.Enter, () => ViewBitboard_Click(this, null));
        InitializeComponent();

    }

    private void ViewBitboard_Click(object? sender, RoutedEventArgs? e)
    {
        var command = BitboardInput.Text?.Trim();
        if (!string.IsNullOrEmpty(command))
        {
            if (!TryExtractBitboardAndShow(command))
            {
                BitboardInput.Text = "Invalid bitboard format. Please enter a valid hex string.";
                return;
            }
            BitboardInput.Text = string.Empty;
        }
    }

    public static bool TryExtractBitboardAndShow(string output)
    {
        var match = HexRegex().Match(output);

        if (!match.Success)
            return false;

        string hexPart = match.Groups[2].Value;
        if (!ulong.TryParse(hexPart, NumberStyles.HexNumber, null, out var value))
            return false;

        var window = new BitboardWindow(value);
        window.Show();
        window.Title += " - " + hexPart;
        return true;
    }

    [GeneratedRegex(@"^(0x)?([0-9a-fA-F]+)$")]
    private static partial Regex HexRegex();
}
