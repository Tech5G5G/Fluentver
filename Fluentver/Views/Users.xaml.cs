using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Principal;
using Windows.ApplicationModel.Calls;
using Windows.ApplicationModel.UserDataAccounts;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fluentver
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Users : Page
    {
        public Users()
        {
            this.InitializeComponent();

            SetUserInfo();
            GetAllUsers();

            usersList.SizeChanged += (object sender, SizeChangedEventArgs e) => usersHeight = (int)this.users.ActualHeight - 48;
        }

        private int usersHeight;

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
            IReadOnlyList<User> users = await User.FindAllAsync();

            int i = 0;
            foreach (User user in users)
            {
                var pictureStream = await user.GetPictureAsync(UserPictureSize.Size1080x1080);
                var openedPictureStream = await pictureStream.OpenReadAsync();
                var image = new BitmapImage();
                image.SetSource(openedPictureStream);

                string displayName = await UserHelper.GetCurrentUserInfoAsync(KnownUserProperties.DisplayName);
                if (string.IsNullOrWhiteSpace(displayName))
                    displayName = Environment.UserName;

                usersList.Children.Add(new UserEntry() { ProfilePicture = image, DisplayName = displayName, AccountName = (string)await user.GetPropertyAsync(KnownUserProperties.AccountName) });

                i++;
            }

            var mw = App.MainWindow;
            mw.UsersWindowHeight = i switch
            {
                2 => mw.UsersWindowHeight + 80,
                >= 3 => mw.UsersWindowHeight + 145,
                _ => mw.UsersWindowHeight + 17,
            };
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

        private void UserInfo_WindowHeight_Increase(Expander sender, ExpanderExpandingEventArgs args)
        {
            MainWindow mw = App.MainWindow;
            if (userInfo.XamlRoot is not null)
                mw.UsersWindowHeight = mw.UsersWindowHeight + 75;
        }

        private void UserInfo_WindowHeight_Decrease(Expander sender, ExpanderCollapsedEventArgs args)
        {
            MainWindow mw = App.MainWindow;
            if (userInfo.XamlRoot is not null)
                mw.UsersWindowHeight = mw.UsersWindowHeight - 75;
        }

        private void Users_WindowHeight_Increase(Expander sender, ExpanderExpandingEventArgs args)
        {
            MainWindow mw = App.MainWindow;
            if (userInfo.XamlRoot is not null)
                mw.UsersWindowHeight = mw.UsersWindowHeight + usersHeight;
        }

        private void Users_WindowHeight_Decrease(Expander sender, ExpanderCollapsedEventArgs args)
        {
            MainWindow mw = App.MainWindow;
            if (userInfo.XamlRoot is not null)
                mw.UsersWindowHeight = mw.UsersWindowHeight - usersHeight;
        }
    }
}
