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

            SetTitleBar(TitleBar);
            ExtendsContentIntoTitleBar = true;
            Title = StringsHelper.GetString("RenamePC.Text");

            SystemBackdrop = SettingValues.Backdrop.Value.ToSystemBackdrop();
            SettingValues.Backdrop.ValueChanged += Backdrop_ValueChanged;

            NameBox.Header = string.Format(StringsHelper.GetString("CurrentName"), SystemHelper.SystemName);
            WindowHelper.ActivateWindow(this.GetWindowHandle());
        }

        private void Backdrop_ValueChanged(object sender, BackdropType e)
        {
            SystemBackdrop = e.ToSystemBackdrop();
        }

        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            NextButton.IsEnabled = SystemHelper.CheckNetBIOSName(NameBox.Text, out var result);
            ErrorText.Text = result switch
            {
                NetBIOSNameCheckResult.ExceedsMaxLength => StringsHelper.GetString("NameTooLong"),
                NetBIOSNameCheckResult.InvalidCharacter => StringsHelper.GetString("NameInvaildCharacters"),
                _ => string.Empty
            };
        }

        private void Cancel(object sender, RoutedEventArgs e) => Close();

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (FinishingScreen.Visibility == Visibility.Visible)
            {
                Close();
                return;
            }

            FinishingScreen.Visibility = Visibility.Visible;
            RenamingScreen.Visibility = Visibility.Collapsed;

            NextButton.Content = StringsHelper.GetString("Finish");
            NextButton.IsEnabled = CancelButton.IsEnabled = false;

            bool renamed = await SystemHelper.RenameSystem(NameBox.Text);

            LoadingRing.Visibility = Visibility.Collapsed;
            ClosingText.Visibility = Visibility.Visible;

            CancelButton.IsEnabled = !(NextButton.IsEnabled = renamed);
            if (!renamed)
                ClosingText.Text = StringsHelper.GetString("ErrorPowerShell");
        }

        private void Name_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (NextButton.IsEnabled && e.Key == Windows.System.VirtualKey.Enter)
                NextButton_Click(sender, e);
        }
    }
}
