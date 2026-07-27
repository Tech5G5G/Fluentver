using System.Diagnostics;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Fluver.Helpers;
using Fluver.UI.Controls;

namespace Fluver.Views
{
    public sealed partial class UsersPage : InfoPage
    {
        private readonly ObservableCollection<UserEntry> _users = [];

        public UsersPage()
        {
            InitializeComponent();
            GetUsers();
        }

        private void GetUsers() => Task.Run(async () =>
        {
            var currentUser = await UserHelper.GetCurrentUserAsync();
            var users = await UserHelper.GetAllUsersAsync();

            DispatcherQueue.TryEnqueue(() =>
            {
                UserPicture.ProfilePicture = currentUser.GetPicture();
                UserPicture.DisplayName = DisplayNameText.Text = currentUser.GetBestDisplayName();
                AccountNameText.Text = currentUser.GetEmailAddress();

                foreach (var user in users)
                {
                    _users.Add(new()
                    {
                        ProfilePicture = user.GetPicture(),
                        DisplayName = user.GetBestDisplayName(),
                        AccountName = user.GetEmailAddress()
                    });
                }

                if (!_users.Any())
                {
                    UserList.Visibility = Visibility.Collapsed;
                    OtherUsersText.Visibility = Visibility.Visible;
                }
            });
        });

        private void UserPicture_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("ms-settings:yourinfo") { UseShellExecute = true });
        }
    }
}
