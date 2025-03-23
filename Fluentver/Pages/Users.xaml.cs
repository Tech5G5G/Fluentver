using Windows.System;

namespace Fluentver.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Users : InfoPage
    {
        public Users()
        {
            this.InitializeComponent();

            SetUserInfo();
            GetAllUsers();
        }

        private async void SetUserInfo()
        {
            string userDisplayName = await UserHelper.GetCurrentUserInfoAsync(KnownUserProperties.DisplayName);
            if (string.IsNullOrWhiteSpace(userDisplayName))
                this.userDisplayName.Text = Environment.UserName;
            else
                this.userDisplayName.Text = userDisplayName;

            userAccountName.Text = await UserHelper.GetCurrentUserInfoAsync(KnownUserProperties.AccountName);
            userPhotoImage.ImageSource = await UserHelper.GetCurrentUserPictureAsync(UserPictureSize.Size1080x1080);
        }

        private async void GetAllUsers()
        {
            int i = 0;
            foreach (User user in await UserHelper.GetUsersAsync())
            {
                var pictureStream = await user.GetPictureAsync(UserPictureSize.Size1080x1080);
                var openedPictureStream = await pictureStream.OpenReadAsync();
                var image = new BitmapImage();
                image.SetSource(openedPictureStream);

                string displayName = (string)await user.GetPropertyAsync(KnownUserProperties.DisplayName);
                if (string.IsNullOrWhiteSpace(displayName))
                    displayName = Environment.UserName;

                usersList.Children.Add(new UserEntry() { ProfilePicture = image, DisplayName = displayName, AccountName = (string)await user.GetPropertyAsync(KnownUserProperties.AccountName) });

                i++;
            }
        }

        private void UserPhoto_PointerEntered(object sender, PointerRoutedEventArgs e) => cameraHover.Visibility = Visibility.Visible;
        private void UserPhoto_PointerPressed(object sender, PointerRoutedEventArgs e) => cameraHoverColor.Fill = new SolidColorBrush(Colors.DarkGray);

        private void UserPhoto_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            cameraHover.Visibility = Visibility.Collapsed;
            cameraHoverColor.Fill = new SolidColorBrush(Colors.Black);
        }

        private void UserPhoto_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            cameraHoverColor.Fill = new SolidColorBrush(Colors.Black);
            Process.Start(new ProcessStartInfo("ms-settings:yourinfo") { UseShellExecute = true });
        }
    }
}
