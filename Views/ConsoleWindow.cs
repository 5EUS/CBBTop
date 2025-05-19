using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace CBBTop.Views
{
    public partial class ConsoleWindow : Window
    {

        public Action<string> Command;
        private List<string> _commandHistory = [];
        private int _historyIndex = -1;

        public ConsoleWindow(Action<string> command)
        {
            Command = command;
            InitializeComponent();
        }

        public void Log(string text)
        {
            ConsoleBox.Text += text + "\n";
            ConsoleBox.CaretIndex = ConsoleBox.Text.Length;
        }

         
        private void CommandInput_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Enter)
            {
                var command = CommandInput.Text?.Trim();
                if (!string.IsNullOrEmpty(command))
                {
                    Command(command);
                    _commandHistory.Add(command);
                    CommandInput.Text = string.Empty;
                    _historyIndex = _commandHistory.Count;
                }
            }
            else if (e.Key == Avalonia.Input.Key.Up)
            {
                if (_commandHistory.Count == 0)
                    return;

                if (_historyIndex > 0)
                {
                    _historyIndex--;
                    CommandInput.Text = _commandHistory[_historyIndex];
                    CommandInput.CaretIndex = CommandInput.Text.Length;
                }
            }
            else if (e.Key == Avalonia.Input.Key.Down)
            {
                if (_commandHistory.Count == 0)
                    return;

                if (_historyIndex < _commandHistory.Count - 1)
                {
                    _historyIndex++;
                    CommandInput.Text = _commandHistory[_historyIndex];
                }
                else
                {
                    _historyIndex = _commandHistory.Count;
                    CommandInput.Text = string.Empty;
                }

                CommandInput.CaretIndex = CommandInput.Text.Length;
            }
        }

    }
}
