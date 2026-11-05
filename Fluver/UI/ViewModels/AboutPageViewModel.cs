using Windows.System;
using Windows.ApplicationModel.DataTransfer;
using CommunityToolkit.Mvvm.Input;
using Fluver.System;
using Fluver.Strings;
using Fluver.Contracts;
using Fluver.Services.Navigation;

namespace Fluver.UI.ViewModels;

public sealed partial class AboutPageViewModel(
    IResources resources,
    IMainPageNavigationService navigation)
    : PageViewModel
{
    public IResources Resources => resources;

    public string Edition => SystemInformation.Edition;
    public string Version => SystemInformation.DisplayVersion ?? string.Empty;
    public string Build => $"{SystemInformation.Version.Build}.{SystemInformation.Version.Revision}";

    public bool IsEvaluationCopy => SystemInformation.EvaluationInfo.IsEvaluationCopy;
    public string ExpirationDateString => SystemInformation.EvaluationInfo.ExpirationDate.ToLocalTime().ToString();

    public string Owner => SystemInformation.Owner ?? string.Empty;

    public string Organization => SystemInformation.Organization ?? string.Empty;
    public bool ShowOrganization => !string.IsNullOrWhiteSpace(SystemInformation.Organization);

    [RelayCommand]
    public static async Task LaunchActivationAsync()
    {
        await Launcher.LaunchUriAsync(new("ms-settings:activation"));
    }

    [RelayCommand]
    public static async Task LaunchWindowsUpdateAsync()
    {
        await Launcher.LaunchUriAsync(new("ms-settings:windowsupdate"));
    }

    [RelayCommand]
    public void CopyOwner()
    {
        DataPackage package = new();
        package.SetText(Owner);

        Clipboard.SetContent(package);
    }

    [RelayCommand]
    public void NavigateUsersPage()
    {
        navigation.Navigate(FluverPage.UsersPage);
    }
}
