using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;
using WinUIEx;
using Fluver.Helpers;
using Fluver.Extensions;

namespace Fluver.Options
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RenamerWindow : UI.Controls.WindowEx
    {
        public RenamerWindow()
        {
            InitializeComponent();
            Closed += (s, e) =>
            {
                App.RenamerWindow = null;
                SettingValues.Backdrop.ValueChanged -= Backdrop_ValueChanged;
            };

            SetTitleBar(titleBar);
            ExtendsContentIntoTitleBar = true;
            Title = StringsHelper.GetString("RenamePC.Text");

            SystemBackdrop = SettingValues.Backdrop.Value.ToSystemBackdrop();
            SettingValues.Backdrop.ValueChanged += Backdrop_ValueChanged;

            name.Header = string.Format(StringsHelper.GetString("CurrentName"), SystemHelper.SystemName);
            WindowHelper.ActivateWindow(this.GetWindowHandle());
        }

        private void Backdrop_ValueChanged(object sender, BackdropType e)
        {
            SystemBackdrop = e.ToSystemBackdrop();
        }

        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            nextButton.IsEnabled = SystemHelper.CheckNetBIOSName(name.Text, out var result);
            error.Text = result switch
            {
                NetBIOSNameCheckResult.ExceedsMaxLength => StringsHelper.GetString("NameTooLong"),
                NetBIOSNameCheckResult.InvalidCharacter => StringsHelper.GetString("NameInvaildCharacters"),
                _ => string.Empty
            };
        }

        private void Cancel(object sender, RoutedEventArgs e) => Close();

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (finishingScreen.Visibility == Visibility.Visible)
            {
                Close();
                return;
            }

            finishingScreen.Visibility = Visibility.Visible;
            renamingScreen.Visibility = Visibility.Collapsed;

            nextButton.Content = StringsHelper.GetString("Finish");
            nextButton.IsEnabled = cancelButton.IsEnabled = false;

            bool renamed = await SystemHelper.RenameSystem(name.Text);

            loadingIndicator.Visibility = Visibility.Collapsed;
            closingText.Visibility = Visibility.Visible;

            cancelButton.IsEnabled = !(nextButton.IsEnabled = renamed);
            if (!renamed)
                closingText.Text = StringsHelper.GetString("ErrorPowerShell");
        }

        private void Name_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (nextButton.IsEnabled && e.Key == Windows.System.VirtualKey.Enter)
                NextButton_Click(sender, e);
        }
    }
}
