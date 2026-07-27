using Microsoft.UI.Xaml;
using Fluver.Options;

namespace Fluver
{
    public sealed partial class App : Application
    {
        public static MainWindow MainWindow { get; set; }

        public static RenamerWindow RenamerWindow { get; set; }

        public App()
        {
            InitializeComponent();
        }

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
    }
}
