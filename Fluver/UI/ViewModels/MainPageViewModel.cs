using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Fluver.Options;
using Fluver.Contracts;
using Fluver.UI.Controls;
using Fluver.Services.Navigation;

namespace Fluver.UI.ViewModels;

public sealed partial class MainPageViewModel : PageViewModel, IDisposable
{
    [ObservableProperty]
    public partial FluverPage SelectedPage { get; set; }

    [ObservableProperty]
    public partial IList<object> ToolbarItems { get; set; }

    private readonly IApplication _app;
    private readonly IWindowManager _manager;
    private readonly IMainPageNavigationService _navigation;

    private readonly ISetting<FluverPage> _startupPage;
    private readonly ISetting<FluverPage> _persistedPage;

    private bool _updatingUi;

    public MainPageViewModel(
        IApplication app,
        IWindowManager manager,
        IMainPageNavigationService navigation,
        [FromKeyedServices(FluverSetting.StartupPage)] ISetting<FluverPage> startupPage,
        [FromKeyedServices(FluverSetting.PersistedPage)] ISetting<FluverPage> persistedPage)
    {
        _app = app;
        _manager = manager;
        _navigation = navigation;
        _startupPage = startupPage;
        _persistedPage = persistedPage;

        navigation.Navigated += OnNavigated;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        _updatingUi = true;
        SelectedPage = _navigation.CurrentPage;
        _updatingUi = false;

        ToolbarItems = e.Content is FluverView { ToolbarItems: { } items } ? items : null;
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

        var page = _startupPage.Value;
        _navigation.Navigate(page == FluverPage.Previous ? _persistedPage.Value : page);
    }

    public void UpdateWindowTitle(string title)
    {
        // TODO: Set to '{SettingsPageName} - Settings - Fluver' when in settings
        // Move responsibility to MainWindowViewModel
        // Make (attached) property for FluverView/page that stores name
        _manager.MainWindow.Title = $"{title} - {_app.DisplayName}";
    }

    [RelayCommand]
    public void Exit()
    {
        _manager.MainWindow.Close();
    }

    void IDisposable.Dispose()
    {
        _persistedPage.Value = _navigation.CurrentPage;
    }
}
