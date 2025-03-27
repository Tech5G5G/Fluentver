using Microsoft.UI.Text;
using Microsoft.UI.Windowing;

namespace Fluentver
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : SizeWindow
    {
        public ObservableCollection<GlyphButton> ToolbarButtons { get; } = [];

        public MainWindow()
        {
            this.InitializeComponent();

            SetTitleBar(titleBar);
            ExtendsContentIntoTitleBar = true;
            AppWindow.SetIcon("Assets/Fluver.ico");

            var presenter = AppWindow.Presenter as OverlappedPresenter;
            presenter.IsMaximizable = presenter.IsMinimizable = presenter.IsResizable = false;

            WindowHelper.SetAppTheme(titleBar.ActualTheme);
            titleBar.ActualThemeChanged += (s, e) => WindowHelper.SetAppTheme(s.ActualTheme);

            Activated += (s, e) => settingsIcon.Foreground = (SolidColorBrush)(e.WindowActivationState == WindowActivationState.Deactivated ?
            Activated += (s, e) => settingsIcon.Style = (Style)(e.WindowActivationState == WindowActivationState.Deactivated ?
            settingsButton.Resources["FontIconTitleBarInactiveStyle"] :
            settingsButton.Resources["FontIconTitleBarStyle"]);

            bar.SetSelectedIndex((int)SettingValues.StartupPage.Value);
            SetWindowsDisplay();
        }

        private void SetWindowsDisplay()
        {
            if (Environment.OSVersion.Version.Build >= 22000)
            {
                windowsLogo.Glyph = "\xE911";
                windowsVersionText.Text = "Windows 11";
            }
            else
            {
                windowsLogo.Glyph = "\xE910";
                windowsVersionText.Text = "Windows 10";
                windowsVersionText.FontWeight = FontWeights.Normal;
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs args) => Close();

        int previousIndex;
        private void Bar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            AppWindow.Title = sender.SelectedItem.Text;
            int currentIndex = sender.GetSelectedIndex();

            ContentFrame.Navigate(currentIndex switch
            {
                1 => typeof(PC),
                2 => typeof(Users),
                3 => typeof(Storage),
                _ => typeof(About),
            },
            this,
            new SlideNavigationTransitionInfo { Effect = previousIndex - currentIndex > 0 ? SlideNavigationTransitionEffect.FromLeft : SlideNavigationTransitionEffect.FromRight });
            previousIndex = currentIndex;
        }

        //TODO: Fix this code
        StackPanel mainContent;
        StackPanel settingsPage;
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainContent is null && settingsPage is null) //Create settings page and set content to it
            {
                mainContent = Content as StackPanel;
                mainContent.Children.Remove(titleBarGrid);

                Content = settingsPage = new()
                {
                    Children =
                    {
                        titleBarGrid,
                        new SettingsPage()
                    }
                };

                settingsIcon.FontSize = 12;
                settingsIcon.Glyph = "\uE72B";
                ToolTipService.SetToolTip(settingsButton, "Back");
            }
            else if (mainContent is null) //Set content to settings page
            {
                mainContent = Content as StackPanel;
                mainContent.Children.Remove(titleBarGrid);

                settingsPage.Children.Insert(0, titleBarGrid);
                Content = settingsPage;

                settingsIcon.FontSize = 12;
                settingsIcon.Glyph = "\uE72B";
                ToolTipService.SetToolTip(settingsButton, "Back");
            }
            else //Reset content to main content
            {
                settingsPage.Children.Remove(titleBarGrid);

                mainContent.Children.Insert(0, titleBarGrid);
                Content = mainContent;
                mainContent = null;

                settingsIcon.FontSize = 14;
                settingsIcon.Glyph = "\uE713";
                ToolTipService.SetToolTip(settingsButton, "Settings");
            }
        }
    }
}
