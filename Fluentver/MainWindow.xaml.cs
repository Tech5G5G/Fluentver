using System.Collections.ObjectModel;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Fluver.Pages;
using Fluver.Options;
using Fluver.Helpers;
using Fluver.Extensions;
using Fluver.UI.Controls;

namespace Fluver
{
    public sealed partial class MainWindow : WindowEx
    {
        public int SelectedIndex
        {
            get => Bar.GetSelectedIndex();
            set => Bar.SetSelectedIndex(value);
        }

        public ObservableCollection<GlyphButton> ToolbarButtons { get; } = [];

        public MainWindow()
        {
            InitializeComponent();
            App.MainWindow = this;

            SetTitleBar(TitleBar);
            ExtendsContentIntoTitleBar = true;
            AppWindow.SetIcon("Assets/Fluver.ico");

            var presenter = AppWindow.Presenter as OverlappedPresenter;
            presenter.IsMaximizable = presenter.IsMinimizable = presenter.IsResizable = false;

            SystemBackdrop = SettingValues.Backdrop.Value.ToSystemBackdrop();
            SettingValues.Backdrop.ValueChanged += (s, e) => SystemBackdrop = e.ToSystemBackdrop();

            WindowHelper.SetAppTheme(TitleBar.ActualTheme);
            TitleBar.ActualThemeChanged += (s, e) => WindowHelper.SetAppTheme(s.ActualTheme);

            Closed += (s, e) => App.RenamerWindow?.Close();
            MinWidth = MaxWidth = 480;

            Activated += (s, e) => settingsIcon.Style = (Style)(e.WindowActivationState == WindowActivationState.Deactivated ?
            settingsButton.Resources["FontIconTitleBarInactiveStyle"] :
            settingsButton.Resources["FontIconTitleBarStyle"]);

            Accelerator.SetOEMAccelerator(Content, 188 /*VK_OEM_COMMA*/, Windows.System.VirtualKey.Control, () => SettingsButton_Click(settingsButton, null));
            SetWindowsDisplay();
            SetupBar();
        }

        private void SetWindowsDisplay()
        {
            if (VersionHelper.IsWindows11)
            {
                WindowsIcon.Glyph = "\xE911";
                WindowsVersionText.Text = "Windows 11";
            }
            else
            {
                WindowsIcon.Glyph = "\xE910";
                WindowsVersionText.Text = "Windows 10";
                WindowsVersionText.FontWeight = Microsoft.UI.Text.FontWeights.Normal;
            }
        }

        private void SetupBar()
        {
            SelectedIndex = (int)SettingValues.StartupPage.Value;
            if (VersionHelper.IsWindowsInsider)
                WipBarItem.Visibility = Visibility.Visible;
            Bar.Loaded += (s, e) =>
            {
                if (Bar.ActualWidth >= 464)
                {
                    BarView.HorizontalScrollMode = ScrollMode.Enabled;
                    BarView.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;

                    BarView.PointerEntered += (s, e) => EnableBarScroller(true, e.Pointer);
                    BarView.PointerExited += (s, e) => EnableBarScroller(false, e.Pointer);
                }
            };
        }

        private void EnableBarScroller(bool enable, Pointer pointer)
        {
            if (pointer.PointerDeviceType == PointerDeviceType.Mouse || !enable)
            {
                BarView.Padding = enable ? new(0, 0, 0, 8) : new();
                BarView.HorizontalScrollBarVisibility = enable ? ScrollBarVisibility.Visible : ScrollBarVisibility.Hidden;
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e) => Close();

        private int _previousIndex;
        private void Bar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs e)
        {
            sender.SelectedItem.StartBringIntoView();
            AppWindow.Title = sender.SelectedItem.Text;
            int currentIndex = sender.GetSelectedIndex();

            ContentFrame.Navigate(
                currentIndex switch
                {
                    1 => typeof(PCPage),
                    2 => typeof(UsersPage),
                    3 => typeof(StoragePage),
                    4 => typeof(InsiderPage),
                    _ => typeof(AboutPage)
                },
                this,
                new SlideNavigationTransitionInfo
                {
                    Effect = _previousIndex - currentIndex > 0 ? SlideNavigationTransitionEffect.FromLeft : SlideNavigationTransitionEffect.FromRight
                });
            _previousIndex = currentIndex;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainContent.Visibility == Visibility.Visible) // Show settings page
            {
                MainContent.Visibility = Visibility.Collapsed;
                SettingsPage.Visibility = Visibility.Visible;

                settingsIcon.FontSize = 12;
                settingsIcon.Glyph = "\uE72B";
                ToolTipService.SetToolTip(SettingsButton, StringsHelper.GetString("SettingsButtonBackTooltip"));
            }
            else // Restore main content
            {
                MainContent.Visibility = Visibility.Visible;
                SettingsPage.Visibility = Visibility.Collapsed;

                settingsIcon.FontSize = 14;
                settingsIcon.Glyph = "\uE713";
                ToolTipService.SetToolTip(SettingsButton, StringsHelper.GetString("SettingsButton.ToolTipService.ToolTip"));
            }
        }

        private void Content_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(sender as UIElement) is PointerPoint { Properties: PointerPointProperties properties } &&
                (properties.IsXButton1Pressed || properties.IsXButton2Pressed)) //Check if XButton pressed
            {
                SelectedIndex = Math.Clamp(SelectedIndex + (properties.IsXButton1Pressed ? -1 : 1), 0, bar.Items.Count);
            }
        }
    }
}
