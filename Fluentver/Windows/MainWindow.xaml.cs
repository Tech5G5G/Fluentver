using Windows.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using WinUIEx;
using Fluver.Strings;
using Fluver.ViewModels;
using Fluver.UI.Controls;
using Fluver.System.Interop;

namespace Fluver.Windows
{
    public sealed partial class MainWindow : FluverWindow
    {
        private const ushort SettingsId = 0x1000;

        public MainWindowViewModel ViewModel { get; }

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();

            Header.OverlapsContent = true;
            Header.DragElement = TitleBar.FindName("DragRegion") as UIElement;

            // The window menu is usually displayed in the OS language, not the language set by Fluver
            // Attempt to use the OS language for consistency
            Menu.AddMenuItem(SettingsId, Text.GetString("SettingsButton/ToolTipService/ToolTip", viewModel.OSCulture))
                .AddMenuSeparator()
                .ItemInvoked += OnMenuItemInvoked;

            (ViewModel = viewModel).InitializeFrame(ContentFrame);
        }

        private void OnMenuItemInvoked(object sender, WindowMenuItemInvokedEventArgs e)
        {
            if (e.ItemId == SettingsId)
            {
                e.Handled = true;
                ViewModel.Settings();
            }
            {
                TitleBar.AnimateSettingsButtonIcon();
        }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(sender as UIElement) is { Properties: { } properties })
            {
                if (properties.IsXButton1Pressed)
                {
                    ViewModel.GoBack();
                }
                else if (properties.IsXButton2Pressed)
                {
                    ViewModel.GoForward();
                }
            }
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.KeyStatus.IsMenuKeyDown)
            {
                if (e.Key == VirtualKey.Left)
                {
                    ViewModel.GoBack();
                }
                else if (e.Key == VirtualKey.Right)
                {
                    ViewModel.GoForward();
                }
            }
        }
    }
}
