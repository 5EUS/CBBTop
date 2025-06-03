using System;
using CBBL.src.Events.Callbacks;
using CBBL.src.Interfaces;
using CBBL.src.Pieces;

namespace CBBTop.Views;

public class MainWindowCallbacks(IBoard board, Action<IBoard> onBitboardMutation) : IBitboardMutationCallback
{
    public IBoard Board { get; } = board;
    public Action<IBoard> OnBitboardMutationAction { get; } = onBitboardMutation;

    public void AfterPieceMoved(PieceType piece, int fromSquare, int toSquare)
    {
    }

    public void BeforePieceMoved(PieceType piece, int fromSquare, int toSquare)
    {
    }

    public void BeforePiecePromoted(PieceType piece, int fromSquare, int toSquare)
    {
    }

    public void BeforePieceRemoved(PieceType piece, int square)
    {
    }

    public void OnBitboardMutation(string mutationType, PieceType? piece, int? fromSquare, int? toSquare, PieceType? promotedTo = null)
    {
        OnBitboardMutationAction?.Invoke(Board);
    }

    public void OnPieceMoved(PieceType piece, int fromSquare, int toSquare)
    {
    }

    public void OnPiecePromoted(PieceType piece, int fromSquare, int toSquare, PieceType promotedTo)
    {
    }

    public void OnPieceRemoved(PieceType piece, int square)
    {
    }
}