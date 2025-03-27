namespace Fluentver.Settings;

public static class SettingValues
{
    public static EnumSetting<Pages> StartupPage { get; } = new(nameof(StartupPage), Pages.About);

    public static Setting<ApplicationDataCompositeValue> ExpanderStates { get; } = new(nameof(ExpanderStates), []);
    public static Setting<ApplicationDataCompositeValue> DiskExpanderStates { get; } = new(nameof(DiskExpanderStates), []);
}
