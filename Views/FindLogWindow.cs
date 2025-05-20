using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;
using Avalonia;
using Avalonia.Interactivity;
using CBBTop.ViewModels;
using Avalonia.Input;
using System.Collections.Generic;
using System;

namespace CBBTop.Views;
public partial class FindLogWindow : Window
{

    private List<string> _logLines;
    private string? _selectedPiece;
    private int _currentSquare;
    private Dictionary<string, Dictionary<int, Dictionary<int, AttackLogParser.AttackEntry>>> _parsedPairs;
    private int _currentIndex = -1;
    private bool _modeByIndex = false;


    public FindLogWindow(List<string> logLines, 
        Dictionary<string, Dictionary<int, Dictionary<int, AttackLogParser.AttackEntry>>> parsedPairs, 
        AttackViewModel viewModel)
    {
        _parsedPairs = parsedPairs;
        _logLines = logLines;
        DataContext = viewModel;
        InitializeComponent();
    }

    private void OnPrev_Click(object? sender, RoutedEventArgs? e)
    {

    }

    private void OnNext_Click(object? sender, RoutedEventArgs? e)
    {

    }

    private void OnShow_Click(object? sender, RoutedEventArgs? e)
    {

        if (PieceSelector.SelectedItem is string piece)
            _selectedPiece = piece;

        _ = int.TryParse(SquareIdInput.Text, out _currentSquare);
        _ = int.TryParse(PermutationInput.Text, out _currentIndex);
        _modeByIndex = ModeSelector.SelectedIndex == 1;    

    }

    private void OnExit_Click(object? sender, RoutedEventArgs? e)
    {
        Close();
    }

    private void Find_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            OnShow_Click(sender, e);
        }
    }
}
