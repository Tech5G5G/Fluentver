namespace Fluver.Windows;

public interface IWindowManager
{
    IWindow MainWindow { get; }

    IReadOnlyList<IWindow> Windows { get; }

    event EventHandler<IWindow> WindowCreated;
    event EventHandler<IWindow> WindowClosed;

    T CreateWindow<T>() where T : IWindow, new();

    void AddWindow(IWindow window);
}
