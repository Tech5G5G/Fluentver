using System.Globalization;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Fluver.Windows;
using Fluver.Navigation;
using Fluver.Globalization;
using Fluver.ApplicationModel;

namespace Fluver.ViewModels;

public sealed partial class MainWindowViewModel(
#pragma warning disable CS9113 // Parameter is unread.
    IWindowManager manager,
    ICultureService culture,
    IBackdropManager backdrop,
    IPackageInformation package,
    IMainPageNavigationService pageNavigation,
    IMainWindowNavigationService windowNavigation)
#pragma warning restore CS9113 // Parameter is unread.
    : ObservableObject
{
    public string DisplayName => package.DisplayName;

    public CultureInfo OSCulture => culture.OSCulture;

    [ObservableProperty]
    public partial bool IsSettingsOpen { get; set; }

    public MainWindowViewModel InitializeWindowManager(MainWindow mainWindow)
    {
        manager.AddWindow(mainWindow);
        return this;
    }

    public MainWindowViewModel InitializeFrame(Frame frame)
    {
        windowNavigation.SetFrame(frame);
        windowNavigation.Navigate(MainWindowPage.MainPage);

        return this;
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

    partial void OnIsSettingsOpenChanged(bool value)
    {
        windowNavigation.Navigate(value ? MainWindowPage.SettingsPage : MainWindowPage.MainPage);
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
