using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using CBBL.src.Board;
using CBBL.src.Interfaces;
using CBBL.src.Pieces;
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
        binder.AddKeyBinding(Key.F3, () => CloseEngine_Click(this, null));
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
        binder.AddKeyBinding(Key.F2, () => OpenEngine_Click(this, null));
        binder.AddKeyBinding(Key.F1, () => NewGame_Click(this, null));

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

    private void NewGame_Click(object? sender, RoutedEventArgs? e)
    {
        var screen = new NewGameWindow(new NewGameViewModel(), ViewModel.EngineManager, DrawChessBoard);
        screen.Show();
    }

    public void DrawChessBoard(IBoard board)
    {
        var boardState = board.State;
        ChessBoard.Children.Clear();
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                bool isWhite = (row + col) % 2 == 0;
                int square = row * 8 + col;
                var border = new Border
                {
                    Background = isWhite ? Brushes.Bisque : Brushes.SaddleBrown,
                    BorderBrush = Brushes.Transparent,
                    BorderThickness = new Thickness(2),
                };

                var type = (PieceType)boardState.GetPieceAtSquare(square);
                if (type != PieceType.None)
                {
                    border.Child = new Image
                    {
                        Source = ViewModel.PieceImages[type].Source,
                        Width = 150,
                        Height = 150,
                        Stretch = Stretch.Uniform,
                    };
                }

                border.Tag = square;
                border.PointerPressed += OnSquareClicked;

                Grid.SetRow(border, 7 - row);
                Grid.SetColumn(border, col);

                ChessBoard.Children.Add(border);
            }
        }
    }

    private int _selectedSquare = 1;
    private Dictionary<Border, IBrush?> _selectedBorders = [];
    private Color selectedColor = new(245, 252, 32, 150);

    public void OnSquareClicked(object? sender, PointerPressedEventArgs? e)
    {
        if (sender is Border border && border.Tag is int square)
        {
            var board = ViewModel.EngineManager.Board;
            var moves = board.State.GetMoves();
            if (_selectedSquare != -1)
            {
                ClearHighlights();
                var type = (PieceType)board.State.GetPieceAtSquare(square);
                var pieces = board.State.ColorToMove == PlayerColor.White
                    ? Pieces.WhitePieces
                    : Pieces.BlackPieces;
                if (pieces.Contains(type))
                    goto newsquare;

                foreach (var move in moves)
                {
                    if (move.FromSquare == _selectedSquare && move.ToSquare == square)
                    {
                        board.ExecuteMove(move);
                        board.State.UpdateMoves();
                        break;
                    }
                }
                _selectedSquare = -1;
                return;
            }

        newsquare:
            _selectedSquare = square;
            if (ChessBoard.Children[square] is Border selectedBorder)
            {
                _selectedBorders.Add(selectedBorder, selectedBorder.Background);
                selectedBorder.Background = new SolidColorBrush(selectedColor);
            }

            foreach (var move in moves)
            {
                if (move.FromSquare == square)
                {
                    if (ChessBoard.Children[move.ToSquare] is Border moveBoarder)
                    {
                        if (_selectedBorders.ContainsKey(moveBoarder))
                            continue;
                        _selectedBorders.Add(moveBoarder, moveBoarder.Background);
                        moveBoarder.Background = new SolidColorBrush(selectedColor);
                    }
                }
            }
        }
    }

    private void ClearHighlights()
    {
        foreach (var border in _selectedBorders.Keys)
        {
            border.Background = _selectedBorders[border];
        }
        _selectedBorders.Clear();
    }

    private void Undo_Click(object? sender, RoutedEventArgs? e)
    {
        ViewModel.EngineManager.Board?.UndoMove();
        ViewModel.EngineManager.Board?.State.UpdateMoves();
    }
}
