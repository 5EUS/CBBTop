using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CBBL.src.Board;
using CBBL.src.Events;
using CBBL.src.Implementation;
using CBBL.src.Interfaces;
using CBBTop.AI;
using CBBTop.ViewModels;

namespace CBBTop.Views;
public partial class NewGameWindow : Window
{
    private readonly NewGameViewModel ViewModel;
    private readonly EngineManager EngineManager;
    private event Action<IBoard> OnMutate;

    public NewGameWindow(ViewModelBase view, EngineManager manager, Action<IBoard> onMutate)
        : base()
    {
        EngineManager = manager;
        DataContext = view;
        OnMutate = onMutate;
        ViewModel = (NewGameViewModel)view;
        InitializeComponent();
    }

    private void Enter_Click(object? sender, RoutedEventArgs e)
    {
        string? fen = FENInput.Text?.Trim();

        int depth = 3;
        if (DepthInput.Text != null && DepthInput.Text.Length > 0)
        {
            if (!int.TryParse(DepthInput.Text, out depth))
            {
                var dialog = new Dialog("Bad depth input!");
                dialog.ShowDialog(this);
                return;
            }
        }

        float posVal = 1;
        if (PositonalValInput.Text != null && PositonalValInput.Text.Length > 0)
        {
            if (!float.TryParse(PositonalValInput.Text, out posVal))
            {
                 var dialog = new Dialog("Bad positional value input!");
                dialog.ShowDialog(this);
                return;
            }
        }

        float pieceVal = 1;
        if (PieceValInput.Text != null && PieceValInput.Text.Length > 0)
        {
            if (!float.TryParse(PieceValInput.Text, out pieceVal))
            {
                 var dialog = new Dialog("Bad piece value input!");
                dialog.ShowDialog(this);
                return;
            }
        }

        PlayerColor color = ColorInput.SelectedIndex == 0 ? PlayerColor.White : PlayerColor.Black;
        PlayerColor opposite = BoardUtils.Opposite(color);

        var board = new CBBLBoard(fen);
        board.State.UpdateMoves();

        EngineManager.Board = board;
        if (depth > 0)
            EngineManager.AIPlayer = new Player(board.State, opposite, depth, pieceVal, posVal);
        // BitboardEventManager.RegisterCallback(new MainWindowCallbacks(board, OnMutate));
        OnMutate(board);
        Close();
    }
}
