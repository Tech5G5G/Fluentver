namespace Fluentver.Settings
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RenamerWindow : Window
    {
        public RenamerWindow()
        {
            this.InitializeComponent();
            Closed += (s, e) => App.RenamerWindow = null;

            SetTitleBar(titleBar);
            ExtendsContentIntoTitleBar = true;
            AppWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);

            name.Header = $"Current name: {SystemHelper.SystemName}";
        }

        private void Name_TextChanged(object sender, TextChangedEventArgs args)
        {
            nextButton.IsEnabled = SystemHelper.CheckNetBIOSName(name.Text, out var result);
            error.Text = result switch
            {
                NetBIOSNameCheckResult.ExceedsMaxLength => "Your PC's name cannot be longer than 15 characters.",
                NetBIOSNameCheckResult.InvalidCharacter => "Your PC's name cannot contain any of the following characters:\n\\ / : * ? < > | or spaces.",
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

            nextButton.Content = "Finish";
            nextButton.IsEnabled = cancelButton.IsEnabled = false;

            bool renamed = await SystemHelper.RenameSystem(name.Text);

            loadingIndicator.Visibility = Visibility.Collapsed;
            closingText.Visibility = Visibility.Visible;

            cancelButton.IsEnabled = !(nextButton.IsEnabled = renamed);
            if (!renamed)
                closingText.Text = "An error occured: Unable to start PowerShell.";
        }

        private void Name_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (nextButton.IsEnabled && e.Key == Windows.System.VirtualKey.Enter)
                NextButton_Click(sender, e);
        }
    }
}
