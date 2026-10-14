using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CBBTop.Views;
public partial class Dialog : Window
{
    public Dialog(string text = "Hello!")
    {
        InitializeComponent();
        TextBox.Text = text;
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
