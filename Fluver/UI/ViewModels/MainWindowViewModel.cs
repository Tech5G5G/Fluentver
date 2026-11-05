using System.Globalization;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Fluver.Strings;
using Fluver.Contracts;
using Fluver.Services.Navigation;

namespace Fluver.UI.ViewModels;

public sealed partial class MainWindowViewModel(
    IResources resources,
    ICultureService culture,
    IPackageInformation package,
    IMainPageNavigationService pageNavigation,
    IMainWindowNavigationService windowNavigation)
    : ObservableObject
{
    public IResources Resources => resources;

    public string DisplayName => package.DisplayName;

    public CultureInfo OSCulture => culture.OSCulture;

    [ObservableProperty]
    public partial bool IsSettingsOpen { get; set; }

    partial void OnIsSettingsOpenChanged(bool value)
    {
        windowNavigation.Navigate(value ? MainWindowPage.SettingsPage : MainWindowPage.MainPage);
    }

    public void InitializeFrame(Frame frame)
    {
        windowNavigation.SetFrame(frame);
        windowNavigation.Navigate(MainWindowPage.MainPage);
    }

    public void GoBack()
    {
        if (windowNavigation.CurrentPage == MainWindowPage.MainPage)
        {
            pageNavigation.GoBack();
        }
        else
        {
            IsSettingsOpen = false;
        }
    }

    public void GoForward()
    {
        if (windowNavigation.CurrentPage == MainWindowPage.MainPage)
        {
            pageNavigation.GoForward();
        }
    }

    public void Settings()
    {
        IsSettingsOpen = true;
    }

    [RelayCommand]
    public void ToggleSettings()
    {
        IsSettingsOpen = !IsSettingsOpen;
    }
}
