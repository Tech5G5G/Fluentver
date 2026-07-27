using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Fluver.Pages;
using Fluver.Options;
using Fluver.Helpers;
using Fluver.Extensions;
using Fluver.ViewModels;
using Fluver.UI.Controls;

namespace Fluver.Windows
{
    public sealed partial class MainWindow : WindowEx
    {
        public MainWindowViewModel ViewModel { get; }

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();

            SetTitleBar(TitleBar);
            ExtendsContentIntoTitleBar = true;
            AppWindow.SetIcon("Assets/Fluver.ico");
            
            if (AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.IsMaximizable = presenter.IsMinimizable = presenter.IsResizable = false;
            }

            SystemBackdrop = SettingValues.Backdrop.Value.ToSystemBackdrop();
            SettingValues.Backdrop.ValueChanged += (s, e) => SystemBackdrop = e.ToSystemBackdrop();

            WindowHelper.SetAppTheme(TitleBar.ActualTheme);
            TitleBar.ActualThemeChanged += (s, e) => WindowHelper.SetAppTheme(s.ActualTheme);

            Closed += (s, e) => App.RenamerWindow?.Close();
            MinWidth = MaxWidth = 480;

            // Activated += (s, e) => settingsIcon.Style = (Style)(e.WindowActivationState == WindowActivationState.Deactivated ?
            // settingsButton.Resources["FontIconTitleBarInactiveStyle"] :
            // settingsButton.Resources["FontIconTitleBarStyle"]);

            (ViewModel = viewModel).InitializeFrame(ContentFrame);
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainContent.Visibility == Visibility.Visible) // Show settings page
            {
                MainContent.Visibility = Visibility.Collapsed;
                SettingsPage.Visibility = Visibility.Visible;

                // settingsIcon.FontSize = 12;
                // settingsIcon.Glyph = "\uE72B";
                ToolTipService.SetToolTip(SettingsButton, StringsHelper.GetString("SettingsButtonBackTooltip"));
            }
            else // Restore main content
            {
                MainContent.Visibility = Visibility.Visible;
                SettingsPage.Visibility = Visibility.Collapsed;

                // settingsIcon.FontSize = 14;
                // settingsIcon.Glyph = "\uE713";
                ToolTipService.SetToolTip(SettingsButton, StringsHelper.GetString("SettingsButton.ToolTipService.ToolTip"));
            }
        }

        private void SettingsButton_Loaded(object sender, RoutedEventArgs e)
        {
            SettingsButton.KeyboardAccelerators.Add(new()
            {
                Modifiers = Windows.System.VirtualKeyModifiers.Control,
                Key = (Windows.System.VirtualKey)188 // VK_OEM_COMMA
            });

            SettingsButton.Loaded -= SettingsButton_Loaded;
        }

        private void SettingsButton_Unloaded(object sender, RoutedEventArgs e)
        {
            SettingsButton.KeyboardAccelerators.Clear();
        }
    }
}
