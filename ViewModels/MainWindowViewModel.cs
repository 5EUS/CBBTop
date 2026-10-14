using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CBBL.src.Interfaces;
using CBBL.src.Pieces;
using CBBTop.Views;

namespace CBBTop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public EngineManager EngineManager { get; private set; }
    public Dictionary<PieceType, Image> PieceImages { get; private set; } = [];
    public ConsoleWindow? ConsoleWindow { get; set; }
    public DisplayBoardInput? DisplayBoardInput { get; set; }

    public MainWindowViewModel()
    {
        EngineManager = new EngineManager();
        EngineManager.OnEngineOutput += (output) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                ConsoleWindow?.Log(output);
                DisplayBoardInput.TryExtractBitboardAndShow(output);
            });
        };
        ConsoleWindow = new ConsoleWindow(EngineManager.Send);
        DisplayBoardInput = new DisplayBoardInput();
        foreach (var pieceType in Pieces.AllPieces)
        {
            var image = new Image
            {
                Source = GetImageForPieceType(pieceType),
                Stretch = Stretch.Uniform
            };
            PieceImages[pieceType] = image;
        }
    }

    public IBoard NewGame()
    {
        return EngineManager.Start();
    }

    private static Bitmap GetImageForPieceType(PieceType pieceType)
    {
        return new Bitmap("Assets/Images/" + pieceType.ToString().ToLowerInvariant() + ".png");
    }
}
