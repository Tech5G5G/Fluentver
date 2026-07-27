using Microsoft.UI.Xaml.Controls;
using Fluver.Views;

namespace Fluver.Navigation;

public sealed class MainWindowNavigationService : NavigationService<MainWindowPage>, IMainWindowNavigationService
{
    public override MainWindowPage Page => Frame.Content switch
    {
        SettingsPage => MainWindowPage.SettingsPage,
        _ => MainWindowPage.MainPage
    };

    protected override IReadOnlyDictionary<MainWindowPage, Type> PageTypes { get; } = new Dictionary<MainWindowPage, Type>
    {
        { MainWindowPage.MainPage, typeof(MainPage) },
        { MainWindowPage.SettingsPage, typeof(SettingsPage) }
    };
}

public enum MainWindowPage
{
    MainPage,
    SettingsPage
}

public interface IMainWindowNavigationService : INavigationService<MainWindowPage>;
