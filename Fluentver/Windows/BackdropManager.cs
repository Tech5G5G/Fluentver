using Windows.UI.ViewManagement;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Fluver.Options;
using Fluver.System.Interop;

namespace Fluver.Windows;

public sealed class BackdropManager : IBackdropManager
{
    private readonly IWindowManager _manager;
    private readonly ISettingsService _settings;

    private readonly UISettings _uiSettings = new();

    private BackdropType _currentBackdrop;

    public BackdropManager(
        IWindowManager manager,
        ISettingsService settings)
    {
        _manager = manager;
        _settings = settings;

        OnColorValuesChanged(_uiSettings, e: null);
        _uiSettings.ColorValuesChanged += OnColorValuesChanged;

        _currentBackdrop = settings.Backdrop.Value;
        settings.Backdrop.ValueChanged += OnValueChanged;

        manager.WindowCreated += OnWindowOpened;
        UpdateWindows();
    }

    private void OnColorValuesChanged(UISettings sender, object e)
    {
        _ = PInvoke.SetPreferredAppMode(sender.GetColorValue(UIColorType.Background) is { R: 0x00, G: 0x00, B: 0x00 }
            ? PreferredAppMode.ForceDark : PreferredAppMode.ForceLight);
        PInvoke.FlushMenuThemes();
    }

    private void OnValueChanged(object sender, BackdropType e)
    {
        _currentBackdrop = e;
        UpdateWindows();
    }

    private void OnWindowOpened(object sender, Window e)
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
