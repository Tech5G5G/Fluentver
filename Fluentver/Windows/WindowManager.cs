using Microsoft.UI.Xaml;

namespace Fluver.Windows;

public sealed partial class WindowManager : IWindowManager
{
    public MainWindow MainWindow => _mainWindow;
    private MainWindow _mainWindow;

    public IReadOnlyList<Window> Windows => _windows;
    private readonly List<Window> _windows = [];

    public T CreateWindow<T>() where T : Window, new()
    {
        T window = new();
        AddWindow(window);
        return window;
    }

    public void AddWindow<T>(T window) where T : Window
    {
        _windows.Add(window);
        window.Closed += OnClosed;

        if (window is MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        void OnClosed(object sender, WindowEventArgs e)
        {
            if (!e.Handled && sender is Window window)
            {
                _windows.Remove(window);
                window.Closed -= OnClosed;

                if (_mainWindow == window)
                {
                    _mainWindow = null;
                }
            }
        }
    }
}
