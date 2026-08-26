using Windows.UI;
using Windows.UI.ViewManagement;
using Fluver.System.Interop;

namespace Fluver.UI;

public sealed class UISettingsService : IUISettingsService
{
    public bool AnimationsEnabled => _settings.AnimationsEnabled;

    public bool DarkModeEnabled => GetColorValue(UIColorType.Background) is { R: 0x00, G: 0x00, B: 0x00 };

    public event EventHandler ColorValuesChanged;

    private readonly UISettings _settings = new();

    public UISettingsService()
    {
        UpdatePreferredAppMode();
        _settings.ColorValuesChanged += OnColorValuesChanged;
    }

    private void OnColorValuesChanged(UISettings sender, object e)
    {
        UpdatePreferredAppMode();
        ColorValuesChanged?.Invoke(sender: this, EventArgs.Empty);
    }

    private void UpdatePreferredAppMode()
    {
        _ = PInvoke.SetPreferredAppMode(DarkModeEnabled ? PreferredAppMode.ForceDark : PreferredAppMode.ForceLight);
        PInvoke.FlushMenuThemes();
    }

    public Color GetColorValue(UIColorType desiredColor)
    {
        return _settings.GetColorValue(desiredColor);
    }
}
