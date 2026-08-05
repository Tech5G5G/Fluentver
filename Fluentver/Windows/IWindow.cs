using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Fluver.Windows;

public interface IWindow
{
    string Title { get; set; }
    bool Visible { get; }
    SystemBackdrop SystemBackdrop { get; set; }

    event TypedEventHandler<object, WindowVisibilityChangedEventArgs> VisibilityChanged;

    event TypedEventHandler<object, WindowActivatedEventArgs> Activated;
    event TypedEventHandler<object, WindowEventArgs> Closed;

    void Activate();
    void Close();
}
