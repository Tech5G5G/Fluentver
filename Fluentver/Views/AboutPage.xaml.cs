using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml;
using Fluver.Helpers;
using Fluver.UI.Controls;

namespace Fluver.Views
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
            UsernameLink.Content = VersionHelper.RegisteredOwner;
            OrgText.Text = VersionHelper.RegisteredOrganization;
            if (string.IsNullOrWhiteSpace(OrgText.Text))
            {
                OrgText.Visibility = Visibility.Collapsed;
            }
        }

        private void SetWindowsInformation()
        {
            EditionText.Text = $"{(VersionHelper.IsWindows11 ? "Windows 11" : "Windows 10")} {VersionHelper.Edition}";
            VersionText.Text = VersionHelper.VersionDisplayName;
            BuildText.Text = $"{VersionHelper.Build}.{VersionHelper.Revision}";

            TrademarkText.Text = Text.Trademark(EditionText.Text);
        }

        private void Navigate_UsersPage(object sender, RoutedEventArgs e)
        {
            App.MainWindow.SelectedIndex = 2;
        }

        private void CopyUsername(object sender, RoutedEventArgs e)
        {
            DataPackage package = new();
            package.SetText((string)UsernameLink.Content);
            Clipboard.SetContent(package);
        }
    }
}
