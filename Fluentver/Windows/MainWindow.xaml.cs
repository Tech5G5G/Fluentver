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
            PInvoke.InsertMenu(menu, PInvoke.SC_CLOSE, PInvoke.MF_BYCOMMAND | PInvoke.MF_STRING, IDM_SETTINGS, StringsHelper.GetString("SettingsButton.ToolTipService.ToolTip"));
            PInvoke.InsertMenu(menu, PInvoke.SC_CLOSE, PInvoke.MF_BYCOMMAND | PInvoke.MF_SEPARATOR, uIDNewItem: 0);

            (ViewModel = viewModel).InitializeWindowManager(mainWindow: this)
                                   .InitializeFrame(ContentFrame);
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

        private void OnActivated(object sender, WindowActivatedEventArgs e)
        {
            TitleBar.IsSettingsButtonActive = e.WindowActivationState != WindowActivationState.Deactivated;
        }

        private void OnClosed(object sender, WindowEventArgs e)
        {
            ViewModel.OnClosed();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // TODO: Animate using DoubleAnimation + DependencyProperties for Width + Height?
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(sender as UIElement) is { Properties: { } properties } &&
                (properties.IsXButton1Pressed || properties.IsXButton2Pressed)) // Check if either XButton is pressed
            {
                ViewModel.OnXButtonPressed(properties.IsXButton1Pressed, properties.IsXButton2Pressed);
            }
        }
    }
}
