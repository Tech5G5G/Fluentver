using Fluver.Helpers;

namespace Fluver.Options;

public sealed class SettingsService : ISettingsService
{
    public ISetting<FluverPage> StartupPage { get; } = new EnumSetting<FluverPage>(nameof(StartupPage), FluverPage.AboutPage);
    public ISetting<BackdropType> Backdrop { get; } = new EnumSetting<BackdropType>(nameof(Backdrop), BackdropType.Mica);

    public SettingsService()
    {
        if (!VersionHelper.IsWindowsInsider)
        {
            if (StartupPage.Value == FluverPage.InsiderPage)
            {
                StartupPage.Value = FluverPage.AboutPage;
            }
        }
    }
}
