using Microsoft.UI.Xaml;

namespace Fluver.Windows;

public sealed partial class WindowManager : IWindowManager
{
    public IWindow MainWindow => _mainWindow;
    private IWindow _mainWindow;

    public IReadOnlyList<IWindow> Windows => _windows;
    private readonly List<IWindow> _windows = [];

    public event EventHandler<IWindow> WindowCreated;
    public event EventHandler<IWindow> WindowClosed;

    public T CreateWindow<T>() where T : IWindow, new()
    {
        T window = new();
        AddWindow(window);
        return window;
    }

    public void AddWindow(IWindow window)
    {
        _windows.Add(window);
        window.Closed += OnClosed;

        if (window is MainWindow)
        {
            _mainWindow = window;
        }

        WindowCreated?.Invoke(sender: this, window);

        void OnClosed(object sender, WindowEventArgs e)
        {
            if (!e.Handled && sender is IWindow window)
            {
                _windows.Remove(window);
                window.Closed -= OnClosed;

                if (_mainWindow == window)
                {
                    _mainWindow = null;
                }

                WindowClosed?.Invoke(sender: this, window);
            }
        }
    }
}
