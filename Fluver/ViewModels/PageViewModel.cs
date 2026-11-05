using Microsoft.UI.Xaml.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Fluver.ViewModels;

public partial class PageViewModel : ObservableObject
{
    public virtual void OnNavigatedTo(NavigationEventArgs e) { }

    public virtual void OnNavigatedFrom(NavigationEventArgs e) { }
}
