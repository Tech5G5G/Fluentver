using Microsoft.UI.Xaml;

namespace Fluver.Windows;

public interface IWindowManager
{
    public MainWindow MainWindow { get; }

    public IReadOnlyList<Window> Windows { get; }

    T CreateWindow<T>() where T : Window, new();

    void AddWindow<T>(T window) where T : Window;
}
