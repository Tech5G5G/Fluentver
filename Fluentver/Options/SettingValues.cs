using Windows.Storage;

namespace Fluver.Options;

public static class SettingValues
{
    public static EnumSetting<Page> StartupPage { get; } = new(nameof(StartupPage), () => Page.About);
    public static EnumSetting<BackdropType> Backdrop { get; } = new(nameof(Backdrop), () => BackdropType.Mica);

    public static Setting<ApplicationDataCompositeValue> ExpanderStates { get; } = new(nameof(ExpanderStates), () => []);
    public static Setting<ApplicationDataCompositeValue> DiskExpanderStates { get; } = new(nameof(DiskExpanderStates), () => []);
}
