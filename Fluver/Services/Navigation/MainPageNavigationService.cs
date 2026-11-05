using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Fluver.Platform;
using Fluver.Contracts;
using Fluver.UI.Views;

namespace Fluver.Services.Navigation;

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

    protected override FluverPage None => FluverPage.None;

    protected override IReadOnlyDictionary<FluverPage, Type> PageTypes { get; } = new Dictionary<FluverPage, Type>
    {
        { FluverPage.AboutPage, typeof(AboutPage) },
        { FluverPage.PCPage, typeof(PCPage) },
        { FluverPage.UsersPage, typeof(UsersPage) },
        { FluverPage.StoragePage, typeof(StoragePage) },
        { FluverPage.InsiderPage, typeof(InsiderPage) }
    };

    protected override NavigationTransitionInfo DetermineTransition(FluverPage oldPage, FluverPage newPage)
    {
        return oldPage == FluverPage.None ?
            new SuppressNavigationTransitionInfo() : // First navigation - setting content of MainPage
            new SlideNavigationTransitionInfo        // Normal navigation
            {
                Effect = oldPage - newPage > 0 ? SlideNavigationTransitionEffect.FromLeft : SlideNavigationTransitionEffect.FromRight
            };
    }

    public override void Navigate(FluverPage page, object parameter)
    {
        base.Navigate(
            page == FluverPage.InsiderPage && !SystemInformation.IsWindowsInsider ? FluverPage.AboutPage : page,
            parameter);
    }

    public override void GoBack()
    {
        var currentPage = CurrentPage;

        if (currentPage != FluverPage.AboutPage)
        {
            base.Navigate(currentPage - 1, parameter: null);
        }
    }

    public override void GoForward()
    {
        var currentPage = CurrentPage;

        if (currentPage != (SystemInformation.IsWindowsInsider ? FluverPage.InsiderPage : FluverPage.StoragePage))
        {
            base.Navigate(currentPage + 1, parameter: null);
        }
    }
}

public enum FluverPage
{
    Previous = -1,
    AboutPage,
    PCPage,
    UsersPage,
    StoragePage,
    InsiderPage,
    None
}
