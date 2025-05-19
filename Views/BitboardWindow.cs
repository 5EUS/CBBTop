using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;
using Avalonia;

namespace CBBTop.Views;
public partial class BitboardWindow : Window
{
    public BitboardWindow(ulong bitboard)
    {
        InitializeComponent();
        DisplayBitboard(bitboard);
    }

    private void DisplayBitboard(ulong bitboard)
    {
        BitboardGrid.Children.Clear();

        for (int rank = 7; rank >= 0; rank--)
        {
            for (int file = 0; file < 8; file++)
            {
                int square = rank * 8 + file;
                bool isSet = (bitboard & (1UL << square)) != 0;

                var cell = new Border
                {
                    Background = isSet ? Brushes.Green : Brushes.Transparent,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };

                BitboardGrid.Children.Add(cell);
            }
        }
    }
}
