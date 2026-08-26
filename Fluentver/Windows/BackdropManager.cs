using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Media;
using Fluver.Options;

namespace Fluver.Windows;

public sealed class BackdropManager : IBackdropManager
{
    private readonly IWindowManager _manager;
    private readonly ISettingsService _settings;

    private BackdropType _currentBackdrop;

    public BackdropManager(
        IWindowManager manager,
        ISettingsService settings)
    {
        _manager = manager;
        _settings = settings;

        _currentBackdrop = settings.Backdrop.Value;
        settings.Backdrop.ValueChanged += OnBackdropValueChanged;

        manager.WindowCreated += OnWindowCreated;
        UpdateWindows();
    }

    private void OnBackdropValueChanged(object sender, BackdropType e)
    {
        _currentBackdrop = e;
        UpdateWindows();
    }

    private void OnWindowCreated(object sender, IWindow e)
    {
        e.SystemBackdrop = CreateBackdrop(_currentBackdrop);
    }

    public void UpdateWindows()
    {
        var windows = _manager.Windows;
        for (int i = 0; i < windows.Count; ++i)
        {
            windows[i].SystemBackdrop = CreateBackdrop(_currentBackdrop);
        }
    }

    private static SystemBackdrop CreateBackdrop(BackdropType backdrop)
    {
        return backdrop switch
        {
            BackdropType.Tabbed => new MicaBackdrop { Kind = MicaKind.BaseAlt },
            BackdropType.Acrylic => new DesktopAcrylicBackdrop(),
            _ => new MicaBackdrop { Kind = MicaKind.Base },
        };
    }
}
