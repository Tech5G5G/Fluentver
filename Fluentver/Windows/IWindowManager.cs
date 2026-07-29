using Microsoft.UI.Xaml;

namespace Fluver.Windows;

public interface IWindowManager
{
    MainWindow MainWindow { get; }

    IReadOnlyList<Window> Windows { get; }

    event EventHandler<Window> WindowCreated;
    event EventHandler<Window> WindowClosed;

    T CreateWindow<T>() where T : Window, new();

    void AddWindow<T>(T window) where T : Window;
}
