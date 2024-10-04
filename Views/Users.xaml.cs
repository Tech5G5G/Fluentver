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

            userPhotoImage.ImageSource = new BitmapImage() { UriSource = new Uri(GetUserPhotoPath()) };
        }

        private async void SetUserInfo()
        {
            userDisplayName.Text = await App.GetCurrentUserInfo(KnownUserProperties.DisplayName);
            userAccountName.Text = await App.GetCurrentUserInfo(KnownUserProperties.AccountName);
        }



        private string GetUserPhotoPath()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();

            string[] files = Directory.GetFiles("C:\\Users\\Public\\AccountPictures\\" + identity.User as string);

            string photoPath = "C:\\ProgramData\\Microsoft\\User Account Pictures\\user.png";
            foreach (string file in files)
            {
                if (file.Contains("1080"))
                {
                    photoPath = file;
                }
            }

            return photoPath;
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
