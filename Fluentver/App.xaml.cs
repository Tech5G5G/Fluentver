namespace Fluentver
{
    public sealed class UserEntry : Control
    {
        public UserEntry()
        {
            this.DefaultStyleKey = typeof(UserEntry);
        }

        public ImageSource ProfilePicture
        {
            get { return (ImageSource)GetValue(ProfilePictureProperty); }
            set { SetValue(ProfilePictureProperty, value); }
        }

        public static readonly DependencyProperty ProfilePictureProperty = DependencyProperty.Register("ProfilePicture", typeof(ImageSource), typeof(UserEntry), new PropertyMetadata(null));

        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register("DisplayName", typeof(string), typeof(UserEntry), new PropertyMetadata(string.Empty));

        public string AccountName
        {
            get { return (string)GetValue(AccountNameProperty); }
            set { SetValue(AccountNameProperty, value); }
        }

        public static readonly DependencyProperty AccountNameProperty = DependencyProperty.Register("AccountName", typeof(string), typeof(UserEntry), new PropertyMetadata(string.Empty));
    }

    public sealed class GlyphButton : Control
    {
        public GlyphButton()
        {
            this.DefaultStyleKey = typeof(GlyphButton);
        }

        public event RoutedEventHandler Click;

        private Button ButtonControl { get; set; }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ButtonControl is not null)
            {
                ButtonControl.Click -= ButtonControl_Click;
            }

            if (GetTemplateChild(nameof(ButtonControl)) is Button button)
            {
                ButtonControl = button;
                ButtonControl.Click += ButtonControl_Click;
            }
        }

        private void ButtonControl_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(GlyphButton), new PropertyMetadata(string.Empty));

        public string Glyph
        {
            get { return (string)GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }

        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(GlyphButton), new PropertyMetadata(string.Empty));
    }

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
            {
            MainWindow = new();
            MainWindow.Activate();
        }

        public static MainWindow MainWindow { get; set; }

        public static Storage StoragePage { get; set; }

        public static PC pcPage { get; set; }
    }
}
