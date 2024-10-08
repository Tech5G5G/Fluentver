using Microsoft.UI;
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
using Fluentver.Views;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fluentver
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private void SetWindowHeight(int height)
        {
            if (AppTitleBar.XamlRoot is not null && this.AppWindow is not null)
            {
                SizeInt32 size;
                size.Width = this.AppWindow.Size.Width;
                size.Height = (int)(height * AppTitleBar.XamlRoot.RasterizationScale);
                this.AppWindow.Resize(size);
            }
        }

        public int AboutWindowHeight { get { return about; } set { about = value; if ((RootNV.SelectedItem as NavigationViewItem).Name == "About_NavItem") SetWindowHeight(value); } }
        private int about = 590;

        public int PCWindowHeight { get { return pc; } set { pc = value; if ((RootNV.SelectedItem as NavigationViewItem).Name == "PC_NavItem") SetWindowHeight(value); } }
        private int pc = 465;

        public int UsersWindowHeight { get { return users; } set { users = value; if ((RootNV.SelectedItem as NavigationViewItem).Name == "Users_NavItem") SetWindowHeight(value); } }
        private int users = 590;

        [DllImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int SetPreferredAppMode(int preferredAppMode);

        public MainWindow()
        {
            this.InitializeComponent();

            SetTitleBar(AppTitleBar);
            ExtendsContentIntoTitleBar = true;
            Title = "About Windows";

            var presenter = this.AppWindow.Presenter as OverlappedPresenter;
            presenter.IsMaximizable = presenter.IsMinimizable = presenter.IsResizable = false;

            this.Activated += MainWindow_Activated;
            AppTitleBar.ActualThemeChanged += AppTitleBar_ActualThemeChanged;
            if (AppTitleBar.ActualTheme == ElementTheme.Dark)
                SetPreferredAppMode(2);

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
                AppTitle.Foreground = (SolidColorBrush)App.Current.Resources["WindowCaptionForegroundDisabled"];
            else
                AppTitle.Foreground = (SolidColorBrush)App.Current.Resources["WindowCaptionForeground"];
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

                SetPreferredAppMode(1);
            }
            else if (currentTheme == ElementTheme.Dark)
            {
                this.AppWindow.TitleBar.ButtonForegroundColor = Colors.White;
                this.AppWindow.TitleBar.ButtonHoverForegroundColor = Colors.White;
                this.AppWindow.TitleBar.InactiveForegroundColor = ((SolidColorBrush)App.Current.Resources["WindowCaptionForegroundDisabled"]).Color;
                this.AppWindow.TitleBar.ButtonPressedForegroundColor = Colors.White;

                SetPreferredAppMode(2);
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs args) => ((App)Application.Current).m_window.Close();

        private void RootNV_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem selectedItem = args.SelectedItem as NavigationViewItem;
            GlyphButton button = new() { Visibility = Visibility.Collapsed };
            GlyphButton button2 = new() { Visibility = Visibility.Collapsed };
            int windowHeight;
            Type page;

            switch (selectedItem.Name)
            {
                default:
                case "About_NavItem":
                    page = typeof(About);
                    button = new GlyphButton() { Name = "activationState", Glyph = "\uEB95", Text = "Activation state" };
                    button.Click += (object sender, RoutedEventArgs e) => Process.Start(new ProcessStartInfo("ms-settings:activation") { UseShellExecute = true });
                    windowHeight = AboutWindowHeight;
                    break;
                case "Users_NavItem":
                    page = typeof(Users);
                    button = new GlyphButton() { Name = "managerUsers", Glyph = "\uE8FA", Text = "Manage other users" };
                    button.Click += (object sender, RoutedEventArgs e) => Process.Start(new ProcessStartInfo("ms-settings:otherusers") { UseShellExecute = true });
                    windowHeight = UsersWindowHeight;
                    break;
                case "PC_NavItem":
                    page = typeof(PC);
                    button = new GlyphButton() { Name = "renamePC", Glyph = "\uE8AC", Text = "Rename your PC" };
                    button.Click += (object sender, RoutedEventArgs e) => Process.Start(new ProcessStartInfo("ms-settings:about") { UseShellExecute = true });
                    button2 = new GlyphButton() { Name = "taskManager", Glyph = "\uE9D9", Text = "Task manager" };
                    button2.Click += (object sender, RoutedEventArgs e) => Process.Start(new ProcessStartInfo("taskmgr") { UseShellExecute = true });
                    windowHeight = PCWindowHeight;
                    break;
            }

            ContentFrame.Navigate(page, null, new EntranceNavigationTransitionInfo());

            SetWindowHeight(windowHeight);
            
            toolbar.Children.Clear();
            toolbar.Children.Add(button);
            toolbar.Children.Add(button2);
        }
    }
}
