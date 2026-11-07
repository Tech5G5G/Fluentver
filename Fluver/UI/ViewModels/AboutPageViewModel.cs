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

    public string Edition => WindowsEnvironment.Edition;
    public string Version => WindowsEnvironment.DisplayVersion ?? string.Empty;
    public string Build => $"{WindowsEnvironment.Version.Build}.{WindowsEnvironment.Version.Revision}";

    public bool IsEvaluationCopy => WindowsEnvironment.EvaluationInfo.IsEvaluationCopy;
    public string ExpirationDateString => WindowsEnvironment.EvaluationInfo.ExpirationDate.ToLocalTime().ToString();

    public string Owner => WindowsEnvironment.Owner ?? string.Empty;

    public string Organization => WindowsEnvironment.Organization ?? string.Empty;
    public bool ShowOrganization => !string.IsNullOrWhiteSpace(WindowsEnvironment.Organization);

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
