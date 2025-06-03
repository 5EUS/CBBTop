using System;
using System.Collections.ObjectModel;
using System.Linq;
using CBBL.src.Board;

namespace CBBTop.ViewModels;

public class NewGameViewModel : ViewModelBase
{
    public ObservableCollection<PlayerColor> AvailableColors { get; } =
        new ObservableCollection<PlayerColor>(Enum.GetValues<PlayerColor>().Cast<PlayerColor>());

    public Action<PlayerColor>? OnPlayerColorSelected { get; set; }
    public Action OnEnterClicked { get; set; } = () => { };

    private PlayerColor _selectedColor;
    public PlayerColor SelectedColor
    {
        get => _selectedColor;
        set => RaiseAndSetIfChanged(ref _selectedColor, value);
    }    

    private void RaiseAndSetIfChanged(ref PlayerColor selectedColor, PlayerColor value)
    {
        OnPlayerColorSelected?.Invoke(value);
        selectedColor = value;
    }
}