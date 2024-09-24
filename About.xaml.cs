using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fluentver
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class About : Page
    {
        public About()
        {
            this.InitializeComponent();

            SetInformation();
            AsyncMethods();
        }

        private async void AsyncMethods()
        {
            string orgName = await ((App)Application.Current).GetCurrentUserInfo(KnownUserProperties.DomainName);
            if (orgName != "" && orgName is not null)
                orgNameText.Content = orgName;
            else
                orgNameText.Visibility = Visibility.Collapsed;

            usernameText.Content = await ((App)Application.Current).GetCurrentUserInfo(KnownUserProperties.AccountName);
        }

        private void SetInformation()
        {
            string HKLMWinNTCurrent = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion";
            string displayName = Registry.GetValue(HKLMWinNTCurrent, "DisplayVersion", "").ToString();
            int build = Environment.OSVersion.Version.Build;
            string version = Registry.GetValue(HKLMWinNTCurrent, "LCUVer", "").ToString();
            string revision = version.Remove(0, 11);
            int currentYear = DateTime.Now.Year;

            copyrightText.Text = "©️ " + currentYear.ToString() + " Microsoft Corporation. All rights reserved.";
            versionText.Text = displayName;
            buildText.Text = build.ToString() + "." + revision;

            string windows;
            if (build >= 22000)
            {
                windows = "Windows 11";
            }
            else
            {
                windows = "Windows 10";
            }

            string productName = Registry.GetValue(HKLMWinNTCurrent, "ProductName", "").ToString();
            string productionEdition = productName.Remove(0, 11);

            editionText.Text = windows + " " + productionEdition;

            prText.Text = "The " + windows + " " + productionEdition + " operating system and its user interface are protected by trademark and other pending or existing intellectual property rights in the United States and other countries/regions.";
        }

        private void WindowsInfo_WindowHeight_Increase(Expander sender, ExpanderExpandingEventArgs args)
        {
            MainWindow mw = (MainWindow)((App)(Application.Current)).m_window;
            if (mw is not null)
            {
                SizeInt32 newSize = new SizeInt32();
                newSize.Width = mw.AppWindow.Size.Width;
                newSize.Height = mw.AppWindow.Size.Height + (int)(90 * windowsInfo.XamlRoot.RasterizationScale);
                mw.AppWindow.Resize(newSize);
            }
        }

        private void WindowsInfo_WindowHeight_Decrease(Expander sender, ExpanderCollapsedEventArgs args)
        {
            MainWindow mw = (MainWindow)((App)(Application.Current)).m_window;
            if (mw is not null)
            {
                SizeInt32 newSize = new SizeInt32();
                newSize.Width = mw.AppWindow.Size.Width;
                newSize.Height = mw.AppWindow.Size.Height - (int)(90 * windowsInfo.XamlRoot.RasterizationScale);
                mw.AppWindow.Resize(newSize);
            }
        }

        private void LegalInfo_WindowHeight_Increase(Expander sender, ExpanderExpandingEventArgs args)
        {
            MainWindow mw = (MainWindow)((App)(Application.Current)).m_window;
            if (mw is not null)
            {
                SizeInt32 newSize = new SizeInt32();
                newSize.Width = mw.AppWindow.Size.Width;
                newSize.Height = mw.AppWindow.Size.Height + (int)(195 * legalInfo.XamlRoot.RasterizationScale);
                mw.AppWindow.Resize(newSize);
            }
        }

        private void LegalInfo_WindowHeight_Decrease(Expander sender, ExpanderCollapsedEventArgs args)
        {
            MainWindow mw = (MainWindow)((App)(Application.Current)).m_window;
            if (mw is not null)
            {
                SizeInt32 newSize = new SizeInt32();
                newSize.Width = mw.AppWindow.Size.Width;
                newSize.Height = mw.AppWindow.Size.Height - (int)(195 * legalInfo.XamlRoot.RasterizationScale);
                mw.AppWindow.Resize(newSize);
            }
        }

        private void Name_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            HyperlinkButton nameText = sender as HyperlinkButton;

            CommandBarFlyout optionsFlyout = new CommandBarFlyout();
            AppBarButton copyButton = new AppBarButton() { Icon = new FontIcon() { Glyph = "\uE8C8" }, Name = (string)nameText.Content };
            AppBarButton openPageButton = new AppBarButton() { Label = "Go to the users page", Icon = new FontIcon() { Glyph = "\uE716" } };

            ToolTipService.SetToolTip(copyButton, "Copy the selected text");

            copyButton.Click += CopyButton_Click;
            openPageButton.Click += Navigate_UsersPage;

            optionsFlyout.PrimaryCommands.Add(copyButton);
            optionsFlyout.SecondaryCommands.Add(openPageButton);

            FlyoutShowOptions myOption = new FlyoutShowOptions();
            myOption.ShowMode = FlyoutShowMode.Transient;
            myOption.Placement = FlyoutPlacementMode.Bottom;

            optionsFlyout.ShowAt(nameText, myOption);
        }

        private void Navigate_UsersPage(object sender, RoutedEventArgs e)
        {
            MainWindow mw = (MainWindow)((App)Application.Current).m_window;
            mw.RootNV.SelectedItem = mw.RootNV.MenuItems.First(i => ((NavigationViewItem)i).Name == "Users_NavItem");
            mw.ContentFrame.Navigate(typeof(Users), null, new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton senderButton = sender as AppBarButton;

            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(senderButton.Name);

            Clipboard.SetContent(dataPackage);
        }
    }
}
