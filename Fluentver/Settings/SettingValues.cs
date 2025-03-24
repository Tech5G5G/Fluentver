namespace Fluentver.Settings;

public static class SettingValues
{
    public static Setting<ApplicationDataCompositeValue> ExpanderStates { get; } = new(nameof(ExpanderStates), []);
}
