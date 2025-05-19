using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CBBTop.ViewModels;


public enum PieceType
{
    None,
    WhitePawn,
    BlackPawn,
    WhiteKnight,
    BlackKnight,
    WhiteBishop,
    BlackBishop,
    WhiteRook,
    BlackRook,
    WhiteQueen,
    BlackQueen,
    WhiteKing,
    BlackKing,
}


public class AttackViewModel : ViewModelBase
{
    public ObservableCollection<PieceType> AvailablePieceTypes { get; } =
        new ObservableCollection<PieceType>(Enum.GetValues<PieceType>().Cast<PieceType>());

    private PieceType _selectedPieceType;
    public PieceType SelectedPieceType
    {
        get => _selectedPieceType;
        set => RaiseAndSetIfChanged(ref _selectedPieceType, value);
    }

    private static void RaiseAndSetIfChanged(ref PieceType selectedPieceType, PieceType value)
    {
        selectedPieceType = value;
    }
}
