namespace Fluentver
{
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

#if !DEBUG
            UnhandledException += (s, e) =>
            {
                e.Handled = true;
                var notification = new Microsoft.Windows.AppNotifications.Builder.AppNotificationBuilder()
                    .AddText("An exception was thrown.")
                    .AddText($"Type: {e.Exception.GetType()}")
                    .AddText($"Message: {e.Message}\r\n" +
                             $"HResult: {e.Exception.HResult}")
                    .BuildNotification();
                Microsoft.Windows.AppNotifications.AppNotificationManager.Default.Show(notification);
            };
#endif
        }

        public static MainWindow MainWindow { get; set; }

        public static RenamerWindow RenamerWindow { get; set; }
    }
}
