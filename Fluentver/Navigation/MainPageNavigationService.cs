using Microsoft.UI.Xaml.Controls;
using Fluver.Views;

namespace Fluver.Navigation;

public sealed class MainPageNavigationService : NavigationService<MainPagePage>, IMainPageNavigationService
{
    public override MainPagePage Page => Frame.Content switch
    {
        PCPage => MainPagePage.PCPage,
        UsersPage => MainPagePage.UsersPage,
        StoragePage => MainPagePage.StoragePage,
        InsiderPage => MainPagePage.InsiderPage,
        _ => MainPagePage.AboutPage
    };

    protected override IReadOnlyDictionary<MainPagePage, Type> PageTypes { get; } = new Dictionary<MainPagePage, Type>
    {
        { MainPagePage.AboutPage, typeof(AboutPage) },
        { MainPagePage.PCPage, typeof(PCPage) },
        { MainPagePage.UsersPage, typeof(UsersPage) },
        { MainPagePage.StoragePage, typeof(StoragePage) },
        { MainPagePage.InsiderPage, typeof(InsiderPage) }
    };
}

public enum MainPagePage
{
    AboutPage,
    PCPage,
    UsersPage,
    StoragePage,
    InsiderPage
}

public interface IMainPageNavigationService : INavigationService<MainPagePage>;
