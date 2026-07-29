using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Fluver.Views;

namespace Fluver.Navigation;

public sealed class MainWindowNavigationService : NavigationService<MainWindowPage>, IMainWindowNavigationService
{
    public override MainWindowPage CurrentPage => Frame.Content switch
    {
        MainPage => MainWindowPage.MainPage,
        SettingsPage => MainWindowPage.SettingsPage,
        _ => MainWindowPage.None
    };

    protected override IReadOnlyDictionary<MainWindowPage, Type> PageTypes { get; } = new Dictionary<MainWindowPage, Type>
    {
        { MainWindowPage.MainPage, typeof(MainPage) },
        { MainWindowPage.SettingsPage, typeof(SettingsPage) }
    };

    protected override NavigationTransitionInfo DetermineTransition(MainWindowPage oldPage, MainWindowPage newPage)
    {
        return oldPage switch
        {
            // First navigation - setting content of MainWindow
            MainWindowPage.None => new SuppressNavigationTransitionInfo(),

            // Navigating to settings page - slide from right (<-)
            MainWindowPage.MainPage when newPage == MainWindowPage.SettingsPage =>
                new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromRight },

            // Navigating from settings page - slide from left (->)
            MainWindowPage.SettingsPage when newPage == MainWindowPage.MainPage =>
                new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromLeft },

            // Unknown state - fall back to default (should never happen tho)
            _ => base.DetermineTransition(oldPage, newPage)
        };
    }
}

public enum MainWindowPage
{
    None,
    MainPage,
    SettingsPage
}

public interface IMainWindowNavigationService : INavigationService<MainWindowPage>;
