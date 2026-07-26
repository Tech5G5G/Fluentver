using Microsoft.UI.Xaml;
using Fluver.Helpers;
using Fluver.UI.Controls;

namespace Fluver.Pages
{
    public sealed partial class InsiderPage : InfoPage
    {
        public InsiderPage()
        {
            InitializeComponent();
            SetVersionInfo();
        }

        private void SetVersionInfo()
        {
            BranchText.Text = VersionHelper.BuildBranch;

            var channel = VersionHelper.Channel;
            ChannelText.Text = StringsHelper.GetString(channel.ToString());
            NotesLink.NavigateUri = new($"https://aka.ms/{channel}latest");

            Task.Run(async () =>
            {
                var user = await VersionHelper.GetWIPAccountAsync();
                string email = user.GetEmailAddress();
                DispatcherQueue.TryEnqueue(() =>
                {
                    AccountText.Text = string.IsNullOrWhiteSpace(email) ? user.GetBestDisplayName() : email;
                    AccountText.Visibility = Visibility.Visible;
                    AccountLoadingRing.Visibility = Visibility.Collapsed;
                });
            });
        }

        private async void FeedbackButton_Click(object sender, RoutedEventArgs e) => await Windows.System.Launcher.LaunchUriAsync(new("feedback-hub:"));
    }
}
