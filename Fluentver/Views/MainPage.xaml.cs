using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Fluver.Helpers;
using Fluver.Options;
using Fluver.Extensions;
using Fluver.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Fluver.ViewModels;

namespace Fluver.Views
{
    public sealed partial class MainPage : ViewModelPage
    {
        public MainPageViewModel ViewModel { get; } = App.Services.GetRequiredService<MainPageViewModel>();

        protected override PageViewModel PageViewModel => ViewModel;

        public int SelectedIndex
        {
            get => Bar.GetSelectedIndex();
            set => Bar.SetSelectedIndex(value);
        }

        public ObservableCollection<GlyphButton> ToolbarButtons { get; } = [];

        public MainPage()
        {
            InitializeComponent();

            SetWindowsDisplay();
            SetupBar();
        }

        // TODO: Replace with IXamlCondition?
        private void SetWindowsDisplay()
        {
            if (VersionHelper.IsWindows11)
            {
                WindowsIcon.Glyph = "\xE911";
                WindowsVersionText.Text = "Windows 11";
            }
            else
            {
                WindowsIcon.Glyph = "\xE910";
                WindowsVersionText.Text = "Windows 10";
                WindowsVersionText.FontWeight = Microsoft.UI.Text.FontWeights.Normal;
            }
        }

        private void SetupBar()
        {
            SelectedIndex = (int)SettingValues.StartupPage.Value;
            if (VersionHelper.IsWindowsInsider)
            {
                WipBarItem.Visibility = Visibility.Visible;
            }
            Bar.Loaded += (s, e) =>
            {
                if (Bar.ActualWidth >= 464)
                {
                    BarView.HorizontalScrollMode = ScrollMode.Enabled;
                    BarView.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;

                    BarView.PointerEntered += (s, e) => EnableBarScroller(true, e.Pointer);
                    BarView.PointerExited += (s, e) => EnableBarScroller(false, e.Pointer);
                }
            };
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
    }
}
