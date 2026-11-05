using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Media;
using Fluver.Options;

namespace Fluver.Windows;

public sealed class BackdropManager : IBackdropManager
{
    private readonly IWindowManager _manager;
    private readonly ISetting<BackdropType> _backdrop;

    private BackdropType _currentBackdrop;

    public BackdropManager(
        IWindowManager manager,
        [FromKeyedServices(FluverSetting.Backdrop)] ISetting<BackdropType> backdrop)
    {
        _manager = manager;
        _backdrop = backdrop;

        _currentBackdrop = backdrop.Value;
        backdrop.ValueChanged += OnBackdropValueChanged;

        UpdateWindows();
        manager.WindowCreated += OnWindowCreated;
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
