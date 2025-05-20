using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CBBTop.Views;

namespace CBBTop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public EngineManager EngineManager { get; private set; }
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
    }
}
