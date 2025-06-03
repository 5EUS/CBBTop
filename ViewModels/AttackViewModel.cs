using System;
using System.Collections.ObjectModel;
using System.Linq;
using CBBL.src.Pieces;

namespace CBBTop.ViewModels;

public class AttackViewModel : ViewModelBase
{
    public ObservableCollection<PieceType> AvailablePieceTypes { get; } =
        new ObservableCollection<PieceType>(Enum.GetValues<PieceType>().Cast<PieceType>());

    public Action<PieceType>? OnPieceSelected { get; set; }

    private PieceType _selectedPieceType;
    public PieceType SelectedPieceType
    {
        get => _selectedPieceType;
        set => RaiseAndSetIfChanged(ref _selectedPieceType, value);
    }

    private void RaiseAndSetIfChanged(ref PieceType selectedPieceType, PieceType value)
    {
        OnPieceSelected?.Invoke(value);
        selectedPieceType = value;
    }
}
