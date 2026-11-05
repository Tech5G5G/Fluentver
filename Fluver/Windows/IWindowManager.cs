namespace Fluver.Windows;

public interface IWindowManager
{
    IReadOnlyList<IWindow> Windows { get; }

    IWindow MainWindow { get; }

    event EventHandler<IWindow> WindowCreated;
    event EventHandler<IWindow> WindowClosed;

    IWindow CreateWindow(object viewModel);
}
