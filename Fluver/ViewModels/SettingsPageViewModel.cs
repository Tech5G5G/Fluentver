using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Fluver.Options;
using Fluver.Strings;
using Fluver.Globalization;
using Fluver.ApplicationModel;

namespace Fluver.ViewModels;

public sealed partial class SettingsPageViewModel(
    IResources resources,
    ICultureService culture,
    IPackageInformation package,
    IApplicationLifetime lifetime,
    [FromKeyedServices(FluverSetting.Backdrop)] ISetting<BackdropType> backdrop,
    [FromKeyedServices(FluverSetting.StartupPage)] ISetting<FluverPage> startupPage)
    : PageViewModel
{
    public IResources Resources => resources;

    public CultureInfo RequestedCulture
    {
        get => culture.RequestedCulture;
        set => IsRestartNeeded = !(culture.RequestedCulture = value).Equals(culture.CurrentCulture);
    }

    public CultureInfo EvaluatedRequestedCulture => culture.EvaluatedRequestedCulture;

    public IReadOnlyCollection<CultureInfo> SupportedCultures => culture.SupportedCultures;

    public ISetting<BackdropType> BackdropSetting => backdrop;
    public ISetting<FluverPage> StartupPageSetting => startupPage;

    public string DisplayName => package.DisplayName;

    public string Version
    {
        get
        {
            if (field is not null)
            {
                return field;
            }

            var version = package.Version;

            var str = $"{version.Major}.{version.Minor}";
            if (version.Build > 0)
            {
                str += $".{version.Build}";
            }

            // Omit revision (Fluver uses semantic versioning)

            return field = str;
        }
    }

    public Uri LatestReleaseUri => new($"https://github.com/Tech5G5G/Fluentver/releases/tag/v{Version}");

    [ObservableProperty]
    public partial bool IsRestartNeeded { get; set; }

    [RelayCommand]
    public void Restart()
    {
        lifetime.Restart();
    }
}
