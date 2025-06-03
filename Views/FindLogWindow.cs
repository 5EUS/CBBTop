using Avalonia.Controls;
using Avalonia.Interactivity;
using CBBTop.ViewModels;
using Avalonia.Input;
using System.Collections.Generic;
using CBBL.src.Pieces;

namespace CBBTop.Views;

public partial class FindLogWindow : Window
{

    private List<string> _logLines;
    private string? _selectedPiece;
    private int _currentSquare;
    private Dictionary<string, Dictionary<int, Dictionary<int, AttackLogParser.AttackEntry>>> _parsedPairs;
    private Dictionary<string, Dictionary<int, Dictionary<int, int>>> _byIndex;
    private int _currentIndex = -1;
    private bool _modeByIndex = true;


    public FindLogWindow(List<string> logLines,
        Dictionary<string, Dictionary<int, Dictionary<int, AttackLogParser.AttackEntry>>> parsedPairs,
        Dictionary<string, Dictionary<int, Dictionary<int, int>>> byIndex,
        AttackViewModel viewModel)
    {
        _parsedPairs = parsedPairs;
        _byIndex = byIndex;
        _logLines = logLines;
        DataContext = viewModel;
        InitializeComponent();
        viewModel.OnPieceSelected += OnPieceSelected;
    }

    private void OnPieceSelected(PieceType type)
    {
        string result;
        if (type == PieceType.WhiteRook || type == PieceType.BlackRook)
        {
            result = "rooks";
        }
        else if (type == PieceType.WhiteBishop || type == PieceType.BlackBishop)
        {
            result = "bishops";
        }
        else
        {
            result = type.ToString();
        }
        _selectedPiece = result;
        PieceSelector.SelectedItem = result;
    }

    private void OnPrev_Click(object? sender, RoutedEventArgs? e)
    {
        if (_currentIndex > 0)
        {
            _currentIndex--;

            ShowCurrent();
        }
    }

    private void OnNext_Click(object? sender, RoutedEventArgs? e)
    {
        if (_currentIndex < _logLines.Count - 1)
        {
            _currentIndex++;
            ShowCurrent();
        }
    }

    private void OnShow_Click(object? sender, RoutedEventArgs? e)
    {

        if (PieceSelector.SelectedItem is string piece)
            _selectedPiece = piece;

        _modeByIndex = ModeSelector.SelectedIndex == 1;

        // _currentIndex = SquareID
        if (_modeByIndex)
            _ = int.TryParse(IndexInput.Text, out _currentIndex);
        else
            _ = int.TryParse(PermutationInput.Text, out _currentIndex);

        ShowCurrent();

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

    private void ShowCurrent()
    {
        if (_selectedPiece == null || _selectedPiece == "None")
        {
            OutputBox.Text = "No piece selected.";
            return;
        }

        if (!_parsedPairs.TryGetValue(_selectedPiece, out var pieceDict))
        {
            OutputBox.Text = $"No data for piece {_selectedPiece}.";
            return;
        }
        if (!pieceDict.TryGetValue(_currentSquare, out var squareDict))
        {
            OutputBox.Text = $"No data for square {_currentSquare}.";
            return;
        }

        int perm = _currentIndex;

        if (_modeByIndex)
        {
            if (_byIndex.TryGetValue(_selectedPiece, out var byIndexDict) &&
                byIndexDict.TryGetValue(_currentSquare, out var indexDict) &&
                indexDict.TryGetValue(_currentIndex, out var resolvedPerm))
            {
                perm = resolvedPerm;
            }
            else
            {
                OutputBox.Text = $"No entry for index {_currentIndex} at square {_currentSquare}.";
                return;
            }
        }

        if (squareDict.TryGetValue(perm, out var entry))
        {
            OutputBox.Text = $"Piece: {_selectedPiece}\nSquare: {_currentSquare}\nPerm: {perm}\n" +
                             $"Blocker: {entry.Blocker}\nAttack: {entry.Attack}\nIndex: {entry.Index}";
        }
        else
        {
            OutputBox.Text = $"No data for perm {perm} at square {_currentSquare}.";
        }
    }
}
