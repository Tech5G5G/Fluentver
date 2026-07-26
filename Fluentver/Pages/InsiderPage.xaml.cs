using Microsoft.UI.Xaml;
using Fluver.Helpers;
using Fluver.UI.Controls;

namespace Fluver.Pages
{
    public sealed partial class InsiderPage : InfoPage
    {
        public InsiderPage()
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
