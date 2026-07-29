using System.Collections;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Fluver.Options;
using Fluver.Windows;
using Fluver.Navigation;

namespace Fluver.ViewModels;

public sealed partial class MainPageViewModel : PageViewModel
{
    [ObservableProperty]
    public partial FluverPage SelectedPage { get; set; }

    [ObservableProperty]
    public partial ICollection ToolBarItems { get; set; }

    private readonly IWindowManager _manager;
    private readonly ISettingsService _settings;
    private readonly IMainPageNavigationService _navigation;

    private bool _updatingUi;

    public MainPageViewModel(
        IWindowManager manager,
        ISettingsService settings,
        IMainPageNavigationService navigation)
    {
        _manager = manager;
        _settings = settings;
        _navigation = navigation;

        navigation.Navigated += OnNavigated;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        _updatingUi = true;
        SelectedPage = _navigation.CurrentPage;
        _updatingUi = false;

        if (e.Content is FluverView { ToolBarItems: { } items })
        {
            ToolBarItems = items;
        }
    }

    partial void OnSelectedPageChanged(FluverPage value)
    {
        if (!_updatingUi)
        {
            _navigation.Navigate(value);
        }
    }

    public void InitializeFrame(Frame frame)
    {
        _navigation.SetFrame(frame);
        _navigation.Navigate(_settings.StartupPage.Value);
    }

    public void UpdateWindowTitle(string title)
    {
        _manager.MainWindow.Title = title + "| Fluver";
    }

    [RelayCommand]
    public void Exit()
    {
        _manager.MainWindow.Close();
    }
}
