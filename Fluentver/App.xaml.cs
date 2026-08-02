using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Fluver.Options;
using Fluver.Windows;
using Fluver.Navigation;
using Fluver.ViewModels;
using Fluver.Globalization;

namespace Fluver
{
    public sealed partial class App : Application
    {
        public static new App Current => (App)Application.Current;

        public static IServiceProvider Services => Current.ServiceProvider;

        public IServiceProvider ServiceProvider => _provider;

        private readonly ServiceProvider _provider;

        public App()
        {
            InitializeComponent();

            ServiceCollection services = new();
            InitializeServices(services);
            _provider = services.BuildServiceProvider();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            new MainWindow(_provider.GetRequiredService<MainWindowViewModel>()).Activate();

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

        private static void InitializeServices(ServiceCollection services)
        {
            services.AddSingleton<IMainPageNavigationService, MainPageNavigationService>()
                    .AddSingleton<IMainWindowNavigationService, MainWindowNavigationService>();

            services.AddSingleton<IWindowManager, WindowManager>()
                    .AddSingleton<IBackdropManager, BackdropManager>()
                    .AddSingleton<ISettingsService, SettingsService>()
                    .AddSingleton<ILanguageService, LanguageService>();

            services.AddTransient<MainPageViewModel>()
                    .AddTransient<MainWindowViewModel>()
                    .AddTransient<RenamerWindowViewModel>();
        }
    }
}
