namespace Fluver.Settings
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RenamerWindow : WindowEx
    {
        public RenamerWindow()
        {
            this.InitializeComponent();
            Closed += (s, e) => App.RenamerWindow = null;

            SetTitleBar(titleBar);
            ExtendsContentIntoTitleBar = true;
            Title = StringsHelper.GetString("RenamePC.Text");

            SystemBackdrop = SettingValues.Backdrop.Value.ToSystemBackdrop();
            SettingValues.Backdrop.ValueChanged += (s, e) =>
            {
                if (App.RenamerWindow == this)
                    SystemBackdrop = e.ToSystemBackdrop();
            };

            name.Header = string.Format(StringsHelper.GetString("CurrentName"), SystemHelper.SystemName);
            WindowHelper.ActivateWindow(this.GetWindowHandle());
        }

        private void Name_TextChanged(object sender, TextChangedEventArgs args)
        {
            nextButton.IsEnabled = SystemHelper.CheckNetBIOSName(name.Text, out var result);
            error.Text = result switch
            {
                NetBIOSNameCheckResult.ExceedsMaxLength => StringsHelper.GetString("NameTooLong"),
                NetBIOSNameCheckResult.InvalidCharacter => StringsHelper.GetString("NameInvaildCharacters"),
                _ => string.Empty
            };
        }

        private void Cancel(object sender, RoutedEventArgs args) => Close();

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
