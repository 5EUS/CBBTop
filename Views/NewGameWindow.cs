using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CBBL.src.Events;
using CBBL.src.Interfaces;
using CBBTop.ViewModels;

namespace CBBTop.Views;
public partial class NewGameWindow : Window
{
    private readonly NewGameViewModel ViewModel;
    private readonly EngineManager EngineManager;
    private readonly Action<IBoard> _onMutate;

    public NewGameWindow(ViewModelBase view, EngineManager manager, Action<IBoard> onMutate)
        : base()
    {
        EngineManager = manager;
        DataContext = view;
        _onMutate = onMutate;
        ViewModel = (NewGameViewModel)view;
        InitializeComponent();
    }

    private void Enter_Click(object? sender, RoutedEventArgs e)
    {
        string fen = FENInput.Text?.Trim() ?? string.Empty;
        IBoard board;
        if (!string.IsNullOrEmpty(fen))
        {
            board = EngineManager.Start(fen);
        }
        else
        {
            board = EngineManager.Start(ViewModel.SelectedColor);
        }
        board.State.UpdateMoves();
        BitboardEventManager.RegisterCallback(new MainWindowCallbacks(board, _onMutate));
        _onMutate(board);
        Close();
    }
}
