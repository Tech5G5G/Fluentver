using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Fluver.ViewModels;
using Fluver.UI.Controls;

namespace Fluver.Windows
{
    public sealed partial class MainWindow : WindowEx
    {
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

            (ViewModel = viewModel).InitializeWindowManager(mainWindow: this)
                                   .InitializeFrame(ContentFrame);
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
