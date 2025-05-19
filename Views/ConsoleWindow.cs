using Avalonia.Controls;

namespace CBBTop.Views
{
    public partial class ConsoleWindow : Window
    {
        public ConsoleWindow()
        {
            InitializeComponent();
        }

        public void Log(string text)
        {
            ConsoleBox.Text += text + "\n";
            ConsoleBox.CaretIndex = ConsoleBox.Text.Length;
        }

        private void CommandInput_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
        }
            
    }
}
