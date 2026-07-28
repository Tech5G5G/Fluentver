using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Fluver.Helpers;
using Fluver.Options;
using Fluver.Extensions;
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

            SetupBar();
            ViewModel.InitializeFrame(ContentFrame);
        }

        private void SetupBar()
        {
            // SelectedIndex = (int)SettingValues.StartupPage.Value;
            if (VersionHelper.IsWindowsInsider)
            {
                // WipBarItem.Visibility = Visibility.Visible;
            }
            // Bar.Loaded += (s, e) =>
            // {
            //     if (Bar.ActualWidth >= 464)
            //     {
            //         BarView.HorizontalScrollMode = ScrollMode.Enabled;
            //         BarView.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            // 
            //         BarView.PointerEntered += (s, e) => EnableBarScroller(true, e.Pointer);
            //         BarView.PointerExited += (s, e) => EnableBarScroller(false, e.Pointer);
            //     }
            // };
        }

        private void EnableBarScroller(bool enable, Pointer pointer)
        {
            if (pointer.PointerDeviceType == PointerDeviceType.Mouse || !enable)
            {
                BarView.Padding = enable ? new(0, 0, 0, 8) : new();
                BarView.HorizontalScrollBarVisibility = enable ? ScrollBarVisibility.Visible : ScrollBarVisibility.Hidden;
            }
        }

        private int _previousIndex;
        private void Bar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs e)
        {
            sender.SelectedItem.StartBringIntoView();
            ViewModel.UpdateWindowTitle(sender.SelectedItem.Text);
            int currentIndex = sender.GetSelectedIndex();

            ContentFrame.Navigate(
                currentIndex switch
                {
                    1 => typeof(PCPage),
                    2 => typeof(UsersPage),
                    3 => typeof(StoragePage),
                    4 => typeof(InsiderPage),
                    _ => typeof(AboutPage)
                },
                parameter: this,
                new SlideNavigationTransitionInfo
                {
                    Effect = _previousIndex - currentIndex > 0 ? SlideNavigationTransitionEffect.FromLeft : SlideNavigationTransitionEffect.FromRight
                });
            _previousIndex = currentIndex;
        }

        private FluverPage ConvertItemToPage(object item)
        {
            // TODO: Can this be fixed?
            var page = (FluverPage)Bar.MenuItems.IndexOf(item);
            ViewModel.SelectedPage = page; // Manually set fsr
            return page;
        }

        private object ConvertPageToItem(FluverPage page)
        {
            return Bar.MenuItems[(int)page];
        }
    }
}
