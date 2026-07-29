namespace Fluver.Options;

public interface ISettingsService
{
    ISetting<FluverPage> StartupPage { get; }
    ISetting<BackdropType> Backdrop { get; }
}
