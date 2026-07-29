using Microsoft.UI.Xaml.Controls;
using Fluver.Views;
using Fluver.Options;

namespace Fluver.Navigation;

public sealed class MainPageNavigationService : NavigationService<FluverPage>, IMainPageNavigationService
{
    public override FluverPage CurrentPage => Frame.Content switch
    {
        AboutPage => FluverPage.AboutPage,
        PCPage => FluverPage.PCPage,
        UsersPage => FluverPage.UsersPage,
        StoragePage => FluverPage.StoragePage,
        InsiderPage => FluverPage.InsiderPage,
        _ => FluverPage.None
    };

    protected override IReadOnlyDictionary<FluverPage, Type> PageTypes { get; } = new Dictionary<FluverPage, Type>
    {
        { FluverPage.AboutPage, typeof(AboutPage) },
        { FluverPage.PCPage, typeof(PCPage) },
        { FluverPage.UsersPage, typeof(UsersPage) },
        { FluverPage.StoragePage, typeof(StoragePage) },
        { FluverPage.InsiderPage, typeof(InsiderPage) }
    };
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

public interface IMainPageNavigationService : INavigationService<FluverPage>;
