using Microsoft.Win32;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.System.Profile;

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

            SetWindowsInformation();
            SetNames();
        }

        private async void SetNames()
        {
            string orgName = await UserHelper.GetCurrentUserInfoAsync(KnownUserProperties.DomainName);
            if (string.IsNullOrWhiteSpace(orgName))
                orgNameText.Visibility = Visibility.Collapsed;
            else
                orgNameText.Content = orgName;

            usernameText.Content = await UserHelper.GetCurrentUserInfoAsync(KnownUserProperties.AccountName);
        }

        private void SetWindowsInformation()
        {
            string HKLMWinNTCurrent = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion";
            ulong deviceFamilyVersion = ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion);

            string displayName = Registry.GetValue(HKLMWinNTCurrent, "DisplayVersion", "").ToString();
            int build = Environment.OSVersion.Version.Build;
            ulong revision = deviceFamilyVersion & 0x000000000000FFFF;
            int currentYear = DateTime.Now.Year;

            copyrightText.Text = "© " + currentYear.ToString() + " Microsoft Corporation. All rights reserved.";
            versionText.Text = displayName;
            buildText.Text = build.ToString() + "." + revision.ToString();

            string windows = build >= 22000 ? "Windows 11" : "Windows 10";

            string productName = Registry.GetValue(HKLMWinNTCurrent, "ProductName", "").ToString();
            string productionEdition = productName.Remove(0, 11);

            editionText.Text = windows + " " + productionEdition;

            prText.Text = "The " + windows + " " + productionEdition + " operating system and its user interface are protected by trademark and other pending or existing intellectual property rights in the United States and other countries/regions.";
        }

        private void Navigate_UsersPage(object sender, RoutedEventArgs e) => App.MainWindow.SelectedIndex = 2;

        private void Navigate_UsersPage(object sender, RoutedEventArgs e)
        {
            MainWindow mw = App.MainWindow;
            //mw.RootNV.SelectedItem = mw.RootNV.MenuItems.First(i => ((NavigationViewItem)i).Name == "Users_NavItem");
            mw.ContentFrame.Navigate(typeof(Users), null, new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromRight });
        }
    }
}
