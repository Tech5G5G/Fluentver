using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;
using Fluver.UI.Controls;
using Fluver.UI.ViewModels;
using Fluver.Services.Navigation;

namespace Fluver.UI.Views
{
    public sealed partial class MainPage : ViewModelPage
    {
        public MainPageViewModel ViewModel { get; } = App.Services.GetRequiredService<MainPageViewModel>();

        protected override PageViewModel PageViewModel => ViewModel;

        public MainPage()
        {
            InitializeComponent();
            ViewModel.InitializeFrame(ContentFrame);
        }

        private void OnBarViewerLoaded(object sender, RoutedEventArgs e)
        {
            const int MaxScrollable = 8;

            BarViewer.Loaded -= OnBarViewerLoaded;

            if (BarViewer.ScrollableWidth > MaxScrollable)
            {
                BarViewer.PointerEntered += OnBarViewerPointerEntered;
                BarViewer.PointerCaptureLost += OnBarViewerPointerCaptureLost;
                BarViewer.PointerExited += OnBarViewerPointerExited;
            }
            else
            {
                VisualStateManager.GoToState(control: this, nameof(BarShort), useTransitions: false);
            }
        }

        private void OnBarViewerPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                VisualStateManager.GoToState(control: this, nameof(BarMouseOver), useTransitions: false);
            }
        }

        private void OnBarViewerPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(BarViewer) is { Position: { } position } &&
                (position.X < 0 || position.X > BarViewer.ActualWidth ||
                position.Y < 0 || position.Y > BarViewer.ActualHeight))
            {
                VisualStateManager.GoToState(control: this, nameof(BarNormal), useTransitions: false);
            }
        }

        private void OnBarViewerPointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(control: this, nameof(BarNormal), useTransitions: false);
        }

        private void OnBarSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs e)
        {
            if (e.SelectedItemContainer is NavigationViewItem { Content: string title } item)
            {
                item.StartBringIntoView();
                ViewModel.UpdateWindowTitle(title);
            }
        }

        private object ConvertPageToItem(FluverPage page)
        {
            return Bar.MenuItems[(int)page];
        }

        private void SetSelectedPage(object item)
        {
            ViewModel.SelectedPage = (FluverPage)Bar.MenuItems.IndexOf(item);
        }
    }
}
