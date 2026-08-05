using Windows.System;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Fluver.Helpers;
using Fluver.ViewModels;
using Fluver.System.Interop;
using WinUIEx;

namespace Fluver.Windows
{
    public sealed partial class MainWindow : UI.Controls.WindowEx
    {
        private const nuint IDM_SETTINGS = 0x0001;

        public MainWindowViewModel ViewModel { get; }

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();

            SetTitleBar(TitleBar.FindName("TitleBar") as UIElement);
            ExtendsContentIntoTitleBar = true;
            AppWindow.SetIcon("Assets/Fluver.ico");

            if (AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.IsMaximizable = presenter.IsMinimizable = presenter.IsResizable = false;
            }

            var menu = PInvoke.GetSystemMenu(this.GetWindowHandle(), bRevert: false);
            // The window menu is usually displayed in the OS language, not the language set by Fluver
            // Attempt to use the OS language for consistency
            NativeInterop.AddMenuItem(menu, IDM_SETTINGS, Text.GetString("SettingsButton/ToolTipService/ToolTip", viewModel.OSCulture));
            NativeInterop.AddMenuSeparator(menu);

            (ViewModel = viewModel).InitializeWindowManager(mainWindow: this)
                                   .InitializeFrame(ContentFrame);
        }

        private void OnActivated(object sender, WindowActivatedEventArgs e)
        {
            TitleBar.IsSettingsButtonActive = e.WindowActivationState != WindowActivationState.Deactivated;
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

        protected override nint Procedure(nint hWnd, uint uMsg, nuint wParam, nint lParam, ref bool handled)
        {
            if (uMsg == 0x0112 && // WM_SYSCOMMAND
                wParam == IDM_SETTINGS)
            {
                ViewModel.Settings();

                handled = true;
                return nint.Zero;
            }

            return base.Procedure(hWnd, uMsg, wParam, lParam, ref handled);
        }
    }
}
