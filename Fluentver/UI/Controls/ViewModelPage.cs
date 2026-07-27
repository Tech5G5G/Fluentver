using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Fluver.ViewModels;

namespace Fluver.UI.Controls;

public abstract partial class ViewModelPage : Page
{
    protected abstract PageViewModel PageViewModel { get; }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        PageViewModel.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        PageViewModel.OnNavigatedFrom(e);
    }
}
