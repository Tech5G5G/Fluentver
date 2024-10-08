﻿using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Windowing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Windows.System;
using Windows.Graphics;
using System.Threading.Tasks;
using Windows.UI.Text;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.ApplicationModel.DataTransfer;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fluentver
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            SetTitleBar(AppTitleBar);
            ExtendsContentIntoTitleBar = true;
            Title = "About Windows";

            var presenter = GetAppWindowAndPresenter();
            presenter.IsMaximizable = presenter.IsMinimizable = presenter.IsResizable = false;

            this.Activated += MainWindow_Activated;
            AppTitleBar.ActualThemeChanged += AppTitleBar_ActualThemeChanged;

            SetWindowsLogo();

            RootNV.SelectedItem = About_NavItem;
        }

        private void SetWindowsLogo()
        {
            int build = Environment.OSVersion.Version.Build;

            string windows;
            if (build >= 22000)
            {
                windows = "Windows 11";
                windowsLogo.Glyph = "\xE911";
            }
            else
            {
                windows = "Windows 10";
                windowsLogo.Glyph = "\xE910";
                windowsVersionText.FontWeight = FontWeights.Normal;
            }
            windowsVersionText.Text = windows;
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
            {
                AppTitle.Foreground = (SolidColorBrush)App.Current.Resources["WindowCaptionForegroundDisabled"];
            }
            else
            {
                AppTitle.Foreground = (SolidColorBrush)App.Current.Resources["WindowCaptionForeground"];
            }
        }

        private void AppTitleBar_ActualThemeChanged(FrameworkElement sender, object args)
        {
            var currentTheme = sender.ActualTheme;

            AppTitle.Foreground = (SolidColorBrush)App.Current.Resources["WindowCaptionForeground"];

            if (currentTheme == ElementTheme.Light)
            {
                this.AppWindow.TitleBar.ButtonForegroundColor = Colors.Black;
                this.AppWindow.TitleBar.ButtonHoverForegroundColor = Colors.Black;
                this.AppWindow.TitleBar.InactiveForegroundColor = ((SolidColorBrush)App.Current.Resources["WindowCaptionForegroundDisabled"]).Color;
                this.AppWindow.TitleBar.ButtonPressedForegroundColor = Colors.Black;
            }
            else if (currentTheme == ElementTheme.Dark)
            {
                this.AppWindow.TitleBar.ButtonForegroundColor = Colors.White;
                this.AppWindow.TitleBar.ButtonHoverForegroundColor = Colors.White;
                this.AppWindow.TitleBar.InactiveForegroundColor = ((SolidColorBrush)App.Current.Resources["WindowCaptionForegroundDisabled"]).Color;
                this.AppWindow.TitleBar.ButtonPressedForegroundColor = Colors.White;
            }
        }

        private OverlappedPresenter GetAppWindowAndPresenter()
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var _apw = AppWindow.GetFromWindowId(myWndId);
            return _apw.Presenter as OverlappedPresenter;
        }

        private void CloseWindow(object sender, RoutedEventArgs args)
        {
            ((App)Application.Current).m_window.Close();
        }

        private void RootNV_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem selectedItem = args.SelectedItem as NavigationViewItem;
            Type page;
            Visibility activationStateVisibility;

            switch (selectedItem.Name)
            {
                case "About_NavItem":
                    page = typeof(About);
                    activationStateVisibility = Visibility.Visible;
                    break;
                case "Users_NavItem":
                    page = typeof(Users);
                    activationStateVisibility = Visibility.Collapsed;
                    break;
                default:
                    page = typeof(About);
                    activationStateVisibility = Visibility.Visible;
                    break;
            }

            ContentFrame.Navigate(page, null, new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromBottom });
            activationState.Visibility = activationStateVisibility;
        }

        private void ActivationState_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("ms-settings:activation") { UseShellExecute = true });
        }
    }
}
