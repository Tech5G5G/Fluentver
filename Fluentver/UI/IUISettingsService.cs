using Windows.UI;
using Windows.UI.ViewManagement;

namespace Fluver.UI;

public interface IUISettingsService
{
    bool AnimationsEnabled { get; }

    bool DarkModeEnabled { get; }

    event EventHandler ColorValuesChanged;

    Color GetColorValue(UIColorType desiredColor);
}
