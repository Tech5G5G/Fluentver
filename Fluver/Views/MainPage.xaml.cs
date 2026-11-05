using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;
using Fluver.Options;
using Fluver.ViewModels;
using Fluver.UI.Controls;

namespace Fluver.Views
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
            const int MaxBarViewerWidth = 475;

            BarViewer.Loaded -= OnBarViewerLoaded;

            if (BarViewer.ExtentWidth <= MaxBarViewerWidth)
            {
                BarViewer.HorizontalScrollMode = ScrollMode.Disabled;
                BarViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }
            else
            {
                BarViewer.Width = double.NaN;
                BarViewer.HorizontalAlignment = HorizontalAlignment.Stretch;

                BarViewer.PointerEntered += OnBarViewerPointerEntered;
                BarViewer.PointerExited += OnBarViewerPointerExited;
            }
        }

        private void OnBarViewerPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ShowBarViewerScrollBar(show: true, e.Pointer.PointerDeviceType);
        }

        private void OnBarViewerPointerExited(object sender, PointerRoutedEventArgs e)
        {
            ShowBarViewerScrollBar(show: false, e.Pointer.PointerDeviceType);
        }

        private void ShowBarViewerScrollBar(bool show, PointerDeviceType pointerType)
        {
            if (pointerType == PointerDeviceType.Mouse || !show)
            {
                BarViewer.Padding = show ? new(left: 0, top: 0, right: 0, bottom: 6) : default;
                BarViewer.HorizontalScrollBarVisibility = show ? ScrollBarVisibility.Visible : ScrollBarVisibility.Hidden;
            }
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
