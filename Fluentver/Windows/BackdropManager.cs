using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Media;
using Fluver.UI;
using Fluver.Options;
using Fluver.System.Interop;

namespace Fluver.Windows;

public sealed class BackdropManager : IBackdropManager
{
    private readonly IWindowManager _manager;
    private readonly ISettingsService _settings;
    private readonly IUISettingsService _uiSettings;

    private BackdropType _currentBackdrop;

    public BackdropManager(
        IWindowManager manager,
        ISettingsService settings,
        IUISettingsService uiSettings)
    {
        _manager = manager;
        _settings = settings;
        _uiSettings = uiSettings;

        OnColorValuesChanged(uiSettings, EventArgs.Empty);
        uiSettings.ColorValuesChanged += OnColorValuesChanged;

        _currentBackdrop = settings.Backdrop.Value;
        settings.Backdrop.ValueChanged += OnValueChanged;

        manager.WindowCreated += OnWindowOpened;
        UpdateWindows();
    }

    private void OnColorValuesChanged(object sender, EventArgs e)
    {
        _ = PInvoke.SetPreferredAppMode((sender as IUISettingsService).DarkModeEnabled ? PreferredAppMode.ForceDark : PreferredAppMode.ForceLight);
        PInvoke.FlushMenuThemes();
    }

    private void OnValueChanged(object sender, BackdropType e)
    {
        _currentBackdrop = e;
        UpdateWindows();
    }

    private void OnWindowOpened(object sender, IWindow e)
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
