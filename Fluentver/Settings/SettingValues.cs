namespace Fluentver.Settings;

public static class SettingValues
{
    public static Setting<ApplicationDataCompositeValue> ExpanderStates { get; } = new(nameof(ExpanderStates), []);
    public static Setting<ApplicationDataCompositeValue> DiskExpanderStates { get; } = new(nameof(DiskExpanderStates), []);
}
