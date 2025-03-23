using Windows.Graphics;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media.Animation;

namespace Fluentver
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
        private static class Brushes
    {
            public static SolidColorBrush WindowCaptionForeground => (SolidColorBrush)Application.Current.Resources["WindowCaptionForeground"];
            public static SolidColorBrush WindowCaptionForegroundDisabled => (SolidColorBrush)Application.Current.Resources["WindowCaptionForegroundDisabled"];
        }

        private void SetWindowHeight(int height)
        {
            if (titleBar.XamlRoot is not null && this.AppWindow is not null)
            {
                SizeInt32 size;
                size.Width = this.AppWindow.Size.Width;
                size.Height = (int)(height * titleBar.XamlRoot.RasterizationScale);
                this.AppWindow.Resize(size);
            }
        }

        public int AboutWindowHeight { get { return about; } set { about = value; if ((RootNV.SelectedItem as NavigationViewItem).Name == "About_NavItem") SetWindowHeight(value); } }
        private int about = 590;

        public int PCWindowHeight { get { return pc; } set { pc = value; if ((RootNV.SelectedItem as NavigationViewItem).Name == "PC_NavItem") SetWindowHeight(value); } }
        private int pc = 465;

        public int UsersWindowHeight { get { return users; } set { users = value; if ((RootNV.SelectedItem as NavigationViewItem).Name == "Users_NavItem") SetWindowHeight(value); } }
        private int users = 428;

        public int StorageWindowHeight { get { return storage; } set { storage = value; if ((RootNV.SelectedItem as NavigationViewItem).Name == "Storage_NavItem") SetWindowHeight(value); } }
        private int storage = 200;

        public MainWindow()
        {
            this.InitializeComponent();

            SetTitleBar(titleBar);
            ExtendsContentIntoTitleBar = true;

            AppWindow.SetIcon("Assets/Fluver.ico");
            Closed += (s, e) => App.pcPage?.StopTimer();

            var presenter = AppWindow.Presenter as OverlappedPresenter;
            presenter.IsMaximizable = presenter.IsMinimizable = presenter.IsResizable = false;

            WindowHelper.SetAppTheme(titleBar.ActualTheme);
            titleBar.ActualThemeChanged += (s, e) => WindowHelper.SetAppTheme(s.ActualTheme);

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

        private void RootNV_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem selectedItem = args.SelectedItem as NavigationViewItem;
            GlyphButton button = new() { Visibility = Visibility.Collapsed };
            GlyphButton button2 = new() { Visibility = Visibility.Collapsed };
            int windowHeight;
            string windowTitle;
            Type page;

            switch (selectedItem.Name)
            {
                default:
                case "About_NavItem":
                    page = typeof(About);
                    button = new GlyphButton() { Name = "activationState", Glyph = "\uEB95", Text = "Activation state" };
                    button.Click += (object sender, RoutedEventArgs e) => Process.Start(new ProcessStartInfo("ms-settings:activation") { UseShellExecute = true });
                    windowHeight = AboutWindowHeight;
                    windowTitle = "About";
                    break;
                case "Users_NavItem":
                    page = typeof(Users);
                    button = new GlyphButton() { Name = "manageUsers", Glyph = "\uE8FA", Text = "Manage other users" };
                    button.Click += (object sender, RoutedEventArgs e) => Process.Start(new ProcessStartInfo("ms-settings:otherusers") { UseShellExecute = true });
                    windowHeight = UsersWindowHeight;
                    windowTitle = "Users";
                    break;
                case "PC_NavItem":
                    page = typeof(PC);
                    button = new GlyphButton() { Name = "renamePC", Glyph = "\uE8AC", Text = "Rename your PC" };
                    button.Click += (object sender, RoutedEventArgs e) => Process.Start(new ProcessStartInfo("ms-settings:about") { UseShellExecute = true });
                    button2 = new GlyphButton() { Name = "taskManager", Glyph = "\uE9D9", Text = "Task manager" };
                    button2.Click += (object sender, RoutedEventArgs e) => Process.Start(new ProcessStartInfo("taskmgr") { UseShellExecute = true });
                    windowHeight = PCWindowHeight;
                    windowTitle = "Your PC";
                    break;
                case "Storage_NavItem":
                    page = typeof(Storage);
                    button = new GlyphButton() { Name = "manageStorage", Glyph = "\uEDA2", Text = "Manage storage" };
                    button.Click += (object sender, RoutedEventArgs e) => Process.Start(new ProcessStartInfo("ms-settings:storagesense") { UseShellExecute = true });
                    button2 = new GlyphButton() { Name = "refreshStorage", Glyph = "\uE72C", Text = "Refresh" };
                    button2.Click += (object sender, RoutedEventArgs e) => App.StoragePage?.Reload();
                    windowHeight = StorageWindowHeight;
                    windowTitle = "Storage";
                    break;
            }

            ContentFrame.Navigate(page, null, new EntranceNavigationTransitionInfo());

            SetWindowHeight(windowHeight);
            AppWindow.Title = windowTitle;
            
            toolbar.Children.Clear();
            toolbar.Children.Add(button);
            toolbar.Children.Add(button2);
        }
    }
}
