using Fluver.ViewModels;
using Fluver.UI.Controls;

namespace Fluver.Windows;

public sealed partial class FluverWindowManager : IWindowManager
{
    public IReadOnlyList<IWindow> Windows => FluverWindow.Windows;

    public IWindow MainWindow => _mainWindow;
    private MainWindow _mainWindow;

    public event EventHandler<IWindow> WindowCreated;
    public event EventHandler<IWindow> WindowClosed;

    public IWindow CreateWindow(object viewModel)
    {
        FluverWindow window = viewModel switch
        {
            MainWindowViewModel main => _mainWindow = new(main),
            RenamerWindowViewModel renamer => new RenamerWindow(renamer),
            _ => throw new NotImplementedException("No window associated with view model type. Should probably add a case...")
        };

        window.Closed += OnClosed;
        WindowCreated?.Invoke(sender: this, window);

        return window;

        void OnClosed(object sender, object e)
        {
            if (sender is FluverWindow window)
            {
                if (window == _mainWindow)
                {
                    _mainWindow = null;
                }

                window.Closed -= OnClosed;
                WindowClosed?.Invoke(sender: this, window);
            }
        }
    }
}
