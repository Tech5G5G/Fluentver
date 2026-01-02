namespace Fluver.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Users : InfoPage
    {
        ObservableCollection<UserEntry> users = [];

        public Users()
        {
            this.InitializeComponent();
            GetUsers();
        }

        private void GetUsers() => Task.Run(async () =>
        {
            var currentUser = await UserHelper.GetCurrentUserAsync();
            var users = await UserHelper.GetAllUsersAsync();

            DispatcherQueue.TryEnqueue(() =>
            {
                userPicture.ProfilePicture = currentUser.GetPicture();
                userPicture.DisplayName = userDisplayName.Text = currentUser.GetBestDisplayName();
                userAccountName.Text = currentUser.GetEmailAddress();

                foreach (var user in users)
                    this.users.Add(new()
                    {
                        ProfilePicture = user.GetPicture(),
                        DisplayName = user.GetBestDisplayName(),
                        AccountName = user.GetEmailAddress()
                    });

                if (!this.users.Any())
                {
                    usersList.Visibility = Visibility.Collapsed;
                    otherUsersLabel.Visibility = Visibility.Visible;
                }
            });
        });

        private void UserPicture_Click(object sender, RoutedEventArgs e) => Process.Start(new ProcessStartInfo("ms-settings:yourinfo") { UseShellExecute = true });
    }
}
