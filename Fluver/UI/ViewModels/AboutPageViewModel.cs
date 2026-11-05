using Windows.System;
using Windows.ApplicationModel.DataTransfer;
using CommunityToolkit.Mvvm.Input;
using Fluver.Strings;
using Fluver.Platform;
using Fluver.Contracts;
using Fluver.Services.Navigation;

namespace Fluver.UI.ViewModels;

public sealed partial class AboutPageViewModel(
    IResources resources,
    IMainPageNavigationService navigation)
    : PageViewModel
{
    public IResources Resources => resources;

    public string Edition => WindowsInfo.Edition;
    public string Version => WindowsInfo.DisplayVersion ?? string.Empty;
    public string Build => $"{WindowsInfo.Version.Build}.{WindowsInfo.Version.Revision}";

    public bool IsEvaluationCopy => WindowsInfo.EvaluationInfo.IsEvaluationCopy;
    public string ExpirationDateString => WindowsInfo.EvaluationInfo.ExpirationDate.ToLocalTime().ToString();

    public string Owner => WindowsInfo.Owner ?? string.Empty;

    public string Organization => WindowsInfo.Organization ?? string.Empty;
    public bool ShowOrganization => !string.IsNullOrWhiteSpace(WindowsInfo.Organization);

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
