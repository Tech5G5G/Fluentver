namespace Fluver.Windows;

public interface IWindowManager
{
    IWindow MainWindow { get; }

    IReadOnlyList<IWindow> Windows { get; }

    event EventHandler<IWindow> WindowCreated;
    event EventHandler<IWindow> WindowClosed;

    IWindow CreateWindow(object viewModel);
    void AddWindow(IWindow window);
}
