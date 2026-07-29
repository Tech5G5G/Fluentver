using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Fluver.Windows;
using Fluver.Navigation;

namespace Fluver.ViewModels;

public sealed partial class MainWindowViewModel(
#pragma warning disable CS9113 // Parameter is unread.
    IWindowManager manager,
    IBackdropManager backdrop,
    IMainPageNavigationService pageNavigation,
    IMainWindowNavigationService windowNavigation)
#pragma warning restore CS9113 // Parameter is unread.
    : ObservableObject
{
    public MainWindowViewModel InitializeWindowManager(MainWindow mainWindow)
    {
        manager.AddWindow(mainWindow);
        return this;
    }

    public MainWindowViewModel InitializeFrame(Frame frame)
    {
        windowNavigation.SetFrame(frame);
        windowNavigation.Navigate(MainWindowPage.MainPage);

        return this;
    }

    public void OnXButtonPressed(bool isXButton1Pressed, bool isXButton2Pressed)
    {
        if (windowNavigation.CurrentPage == MainWindowPage.MainPage)
        {
            if (isXButton1Pressed)
            {
                pageNavigation.GoBack();
            }
            else
            {
                pageNavigation.GoForward();
            }
        }
        else
        {
            // TODO: Navigate thru settings.
        }
    }

    public void OnClosed()
    {
        var windows = manager.Windows;
        for (int i = 0; i < windows.Count; ++i)
        {
            windows[i].Close();
        }
    }
}
