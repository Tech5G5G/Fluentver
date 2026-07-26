using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml;
using Fluver.Helpers;
using Fluver.UI.Controls;

namespace Fluver.Pages
{
    public sealed partial class AboutPage : InfoPage
    {
        public AboutPage()
        {
            InitializeComponent();

            SetNames();
            SetWindowsInformation();
        }

        private void SetNames()
        {
            username.Content = VersionHelper.RegisteredOwner;
            orgName.Text = VersionHelper.RegisteredOrganization;
            if (string.IsNullOrWhiteSpace(orgName.Text)) orgName.Visibility = Visibility.Collapsed;
        }

        private void SetWindowsInformation()
        {
            editionText.Text = $"{(VersionHelper.IsWindows11 ? "Windows 11" : "Windows 10")} {VersionHelper.Edition}";
            versionText.Text = VersionHelper.VersionDisplayName;
            buildText.Text = $"{VersionHelper.Build}.{VersionHelper.Revision}";

            trademark.Text = string.Format(StringsHelper.GetString("Trademark"), editionText.Text);
            copyright.Text = string.Format(StringsHelper.GetString("Copyright"), DateTime.Now.Year);
        }

        private void Navigate_UsersPage(object sender, RoutedEventArgs e) => App.MainWindow.SelectedIndex = 2;

        private void CopyUsername(object sender, RoutedEventArgs e)
        {
            DataPackage package = new();
            package.SetText((string)username.Content);
            Clipboard.SetContent(package);
        }
    }
}
