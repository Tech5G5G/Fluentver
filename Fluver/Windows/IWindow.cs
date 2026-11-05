using Microsoft.UI.Xaml.Media;

namespace Fluver.Windows;

public interface IWindow
{
    string Title { get; set; }
    bool IsActive { get; }
    IWindow Owner { get; set; }

    SystemBackdrop SystemBackdrop { get; set; }

    event EventHandler Opened;
    event EventHandler Closed;

    void Show();
    void Close();
}
