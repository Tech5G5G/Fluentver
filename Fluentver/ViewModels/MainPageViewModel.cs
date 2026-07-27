using Fluver.Windows;

namespace Fluver.ViewModels;

public sealed partial class MainPageViewModel(IWindowManager manager) : PageViewModel
{
    public void UpdateWindowTitle(string title)
    {
        manager.MainWindow.Title = title;
    }
}
