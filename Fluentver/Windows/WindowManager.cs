using Fluver.ViewModels;

namespace Fluver.Windows;

public sealed partial class WindowManager : IWindowManager
{
    public IWindow MainWindow => _mainWindow;
    private IWindow _mainWindow;

    public IReadOnlyList<IWindow> Windows => _windows;
    private readonly List<IWindow> _windows = [];

    public event EventHandler<IWindow> WindowCreated;
    public event EventHandler<IWindow> WindowClosed;

    public IWindow CreateWindow(object viewModel)
    {
        IWindow window = viewModel switch
        {
            MainWindowViewModel main => new MainWindow(main),
            RenamerWindowViewModel renamer => new RenamerWindow(renamer),
            _ => null
        };

        if (window is not null)
        {
            AddWindow(window);
        }

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

        void OnClosed(object sender, object e)
        {
            if (sender is IWindow window)
            {
                _windows.Remove(window);
                window.Closed -= OnClosed;

                if (window == _mainWindow)
                {
                    _mainWindow = null;
                }

                WindowClosed?.Invoke(sender: this, window);
            }
        }
    }
}
