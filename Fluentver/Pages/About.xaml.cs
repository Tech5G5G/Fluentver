using Windows.ApplicationModel.DataTransfer;

namespace Fluentver.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class About : InfoPage
    {
        public About()
        {
            this.InitializeComponent();

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

            trademark.Text = $"The {editionText.Text} operating system and its user interface are protected by trademark and other pending or existing intellectual property rights in the United States and other countries/regions.";
            copyright.Text = $"© {DateTime.Now.Year} Microsoft Corporation. All rights reserved.";
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
