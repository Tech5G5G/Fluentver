using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Fluver.Navigation;

namespace Fluver.ViewModels;

public sealed partial class MainWindowViewModel(IMainWindowNavigationService navigation) : ObservableObject
{
    public void InitializeFrame(Frame frame)
    {
        navigation.SetFrame(frame);
        navigation.Navigate(MainWindowPage.MainPage, transition: Transition.Suppress);
    }
}
