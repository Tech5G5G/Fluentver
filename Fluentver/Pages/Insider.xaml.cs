namespace Fluver.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Insider : InfoPage
    {
        public Insider()
        {
            this.InitializeComponent();
            SetVersionInfo();
        }

        private void SetVersionInfo()
        {
            branch.Text = VersionHelper.BuildBranch;

            var channel = VersionHelper.Channel;
            this.channel.Text = StringsHelper.GetString(channel.ToString());
            notesLink.NavigateUri = new($"https://aka.ms/{channel}latest");

            Task.Run(async () =>
            {
                var user = await VersionHelper.GetWIPAccountAsync();
                string email = user.GetEmailAddress();
                DispatcherQueue.TryEnqueue(() =>
                {
                    account.Text = string.IsNullOrWhiteSpace(email) ? user.GetBestDisplayName() : email;
                    account.Visibility = Visibility.Visible;
                    accountLoading.Visibility = Visibility.Collapsed;
                });
            });
        }

        private async void FeedbackButton_Click(object sender, RoutedEventArgs e) => await Windows.System.Launcher.LaunchUriAsync(new("feedback-hub:"));
    }
}
