using Windows.System;
using Microsoft.UI.Xaml;
using Fluver.Helpers;
using Fluver.Platform;
using Fluver.UI.Controls;

namespace Fluver.UI.Views
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
            BranchText.Text = WindowsInfo.BuildBranch;

            var channel = WindowsInfo.InsiderChannel;
            ChannelText.Text = Strings.Resources.GetString(channel.ToString());
            NotesLink.NavigateUri = new($"https://aka.ms/{channel}latest");

            Task.Run(async () =>
            {
                var user = await WindowsInfo.GetWIPAccountAsync();
                string email = user.GetEmailAddress();
                DispatcherQueue.TryEnqueue(() =>
                {
                    AccountText.Text = string.IsNullOrWhiteSpace(email) ? user.GetBestDisplayName() : email;
                    AccountText.Visibility = Visibility.Visible;
                    AccountLoadingRing.Visibility = Visibility.Collapsed;
                });
            });
        }

        private void FeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            _ = Launcher.LaunchUriAsync(new("feedback-hub:"));
        }
    }
}
