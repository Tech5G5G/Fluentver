using Microsoft.UI.Text;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media.Animation;

namespace Fluentver
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : SizeWindow
    {
        private static class Brushes
        {
            public static SolidColorBrush WindowCaptionForeground => (SolidColorBrush)Application.Current.Resources["WindowCaptionForeground"];
            public static SolidColorBrush WindowCaptionForegroundDisabled => (SolidColorBrush)Application.Current.Resources["WindowCaptionForegroundDisabled"];
        }

        public ObservableCollection<GlyphButton> ToolbarButtons { get; } = [];

        public MainWindow()
        {
            this.InitializeComponent();

            SetTitleBar(titleBar);
            ExtendsContentIntoTitleBar = true;
            AppWindow.SetIcon("Assets/Fluver.ico");

            var presenter = AppWindow.Presenter as OverlappedPresenter;
            presenter.IsMaximizable = presenter.IsMinimizable = presenter.IsResizable = false;

            WindowHelper.SetAppTheme(titleBar.ActualTheme);
            titleBar.ActualThemeChanged += (s, e) => WindowHelper.SetAppTheme(s.ActualTheme);

            SizeToElement(Content as FrameworkElement, Dimensions.Height);
            bar.SetSelectedIndex(0);

            SetWindowsDisplay();
        }

        private void SetWindowsDisplay()
        {
            if (Environment.OSVersion.Version.Build >= 22000)
            {
                windowsLogo.Glyph = "\xE911";
                windowsVersionText.Text = "Windows 11";
            }
            else
            {
                windowsLogo.Glyph = "\xE910";
                windowsVersionText.Text = "Windows 10";
                windowsVersionText.FontWeight = FontWeights.Normal;
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs args) => Close();

        int previousIndex;
        private void Bar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            AppWindow.Title = sender.SelectedItem.Text;
            int currentIndex = sender.GetSelectedIndex();

            ContentFrame.Navigate(currentIndex switch
            {
                1 => typeof(PC),
                2 => typeof(Users),
                3 => typeof(Storage),
                _ => typeof(About),
            },
            this,
            new SlideNavigationTransitionInfo { Effect = previousIndex - currentIndex > 0 ? SlideNavigationTransitionEffect.FromLeft : SlideNavigationTransitionEffect.FromRight });
            previousIndex = currentIndex;
        }
    }
}
