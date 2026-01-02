namespace Fluentver
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        public int SelectedIndex
        {
            get => bar.GetSelectedIndex();
            set => bar.SetSelectedIndex(value);
        }

        public ObservableCollection<GlyphButton> ToolbarButtons { get; } = [];

        public MainWindow()
        {
            this.InitializeComponent();
            App.MainWindow = this;

            SetTitleBar(titleBar);
            ExtendsContentIntoTitleBar = true;
            AppWindow.SetIcon("Assets/Fluver.ico");

            var presenter = AppWindow.Presenter as OverlappedPresenter;
            presenter.IsMaximizable = presenter.IsMinimizable = presenter.IsResizable = false;

            SystemBackdrop = SettingValues.Backdrop.Value.ToSystemBackdrop();
            SettingValues.Backdrop.ValueChanged += (s, e) => SystemBackdrop = e.ToSystemBackdrop();

            WindowHelper.SetAppTheme(titleBar.ActualTheme);
            titleBar.ActualThemeChanged += (s, e) => WindowHelper.SetAppTheme(s.ActualTheme);

            Closed += (s, e) => App.RenamerWindow?.Close();
            PositionChanged += (s, e) => Width = 480;

            Activated += (s, e) => settingsIcon.Style = (Style)(e.WindowActivationState == WindowActivationState.Deactivated ?
            settingsButton.Resources["FontIconTitleBarInactiveStyle"] :
            settingsButton.Resources["FontIconTitleBarStyle"]);

            Accelerator.SetOEMAccelerator(Content, 188 /*VK_OEM_COMMA*/, Windows.System.VirtualKey.Control, () => SettingsButton_Click(null, null));
            SetWindowsDisplay();
            SetupBar();
        }

        private void SetWindowsDisplay()
        {
            if (VersionHelper.IsWindows11)
            {
                windowsLogo.Glyph = "\xE911";
                windowsVersionText.Text = "Windows 11";
            }
            else
            {
                windowsLogo.Glyph = "\xE910";
                windowsVersionText.Text = "Windows 10";
                windowsVersionText.FontWeight = Microsoft.UI.Text.FontWeights.Normal;
            }
        }

        private void SetupBar()
        {
            SelectedIndex = (int)SettingValues.StartupPage.Value;
            if (VersionHelper.IsWindowsInsider)
                wipItem.Visibility = Visibility.Visible;
            bar.Loaded += (s, e) =>
            {
                if (bar.ActualWidth >= 464)
                {
                    barScroller.HorizontalScrollMode = ScrollMode.Enabled;
                    barScroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;

                    barScroller.PointerEntered += (s, e) => EnableBarScroller(true, e.Pointer);
                    barScroller.PointerExited += (s, e) => EnableBarScroller(false, e.Pointer);
                }
            };
        }

        private void EnableBarScroller(bool enable, Pointer pointer)
        {
            if (pointer.PointerDeviceType == PointerDeviceType.Mouse || !enable)
            {
                barScroller.Padding = enable ? new(0, 0, 0, 8) : new();
                barScroller.HorizontalScrollBarVisibility = enable ? ScrollBarVisibility.Visible : ScrollBarVisibility.Hidden;
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs args) => Close();

        int previousIndex;
        private void Bar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            sender.SelectedItem.StartBringIntoView();
            AppWindow.Title = sender.SelectedItem.Text;
            int currentIndex = sender.GetSelectedIndex();

            ContentFrame.Navigate(currentIndex switch
            {
                1 => typeof(PC),
                2 => typeof(Users),
                3 => typeof(Storage),
                4 => typeof(Insider),
                _ => typeof(About)
            },
            this,
            new SlideNavigationTransitionInfo { Effect = previousIndex - currentIndex > 0 ? SlideNavigationTransitionEffect.FromLeft : SlideNavigationTransitionEffect.FromRight });
            previousIndex = currentIndex;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainContent.Visibility == Visibility.Visible) //Show settings page
            {
                mainContent.Visibility = Visibility.Collapsed;
                settingsPage.Visibility = Visibility.Visible;

                settingsIcon.FontSize = 12;
                settingsIcon.Glyph = "\uE72B";
                ToolTipService.SetToolTip(settingsButton, StringsHelper.GetString("SettingsButtonBackTooltip"));
            }
            else //Restore main content
            {
                mainContent.Visibility = Visibility.Visible;
                settingsPage.Visibility = Visibility.Collapsed;

                settingsIcon.FontSize = 14;
                settingsIcon.Glyph = "\uE713";
                ToolTipService.SetToolTip(settingsButton, StringsHelper.GetString("SettingsButton.ToolTipService.ToolTip"));
            }
        }

        private void Content_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(sender as UIElement) is PointerPoint { Properties: PointerPointProperties properties } &&
                (properties.IsXButton1Pressed || properties.IsXButton2Pressed)) //Check if XButton pressed
            {
                SelectedIndex = Math.Clamp(SelectedIndex + (properties.IsXButton1Pressed ? -1 : 1), 0, bar.Items.Count);
            }
        }
    }
}
