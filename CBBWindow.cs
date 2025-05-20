using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace CBBTop;

public abstract class CBBWindow : Window
{
    protected async Task<string> OpenWindowGetFilePath(string title)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Executable")
                {
                    Patterns = ["*.exe", "*.bin", "*.out"]
                },
                new FilePickerFileType("Logs")
                {
                    Patterns = ["*.log", "*.txt", "*.bin"]
                },
                new FilePickerFileType("All Files")
                {
                    Patterns = ["*", "*.*"]
                }
            ]
        });

        if (files.Any())
        {
            var file = files[0];
            var path = file.Path?.LocalPath;

            if (!string.IsNullOrEmpty(path))
            {
                return path;
            }
        } 
        return string.Empty;
    }
}