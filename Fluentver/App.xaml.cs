using Microsoft.Extensions.DependencyInjection;
using Microsoft.Windows.AppLifecycle;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Fluver.Options;
using Fluver.Windows;
using Fluver.Navigation;
using Fluver.ViewModels;
using Fluver.Globalization;
using Fluver.ApplicationModel;

namespace Fluver
{
    public sealed partial class App : Application, IAppLifetime, IDisposable
    {
        public static new App Current => (App)Application.Current;

        public static IServiceProvider Services => Current.ServiceProvider;

        public IServiceProvider ServiceProvider => _provider;

        private readonly ServiceProvider _provider;

        public App()
        {
            InitializeComponent();
            DispatcherQueue.GetForCurrentThread().ShutdownStarting += OnShutdownStarting;

            ServiceCollection services = new();
            InitializeServices(services);
            _provider = services.BuildServiceProvider();

            // Make sure culture info is set properly
            _provider.GetRequiredService<ICultureService>();
        }

        private void OnShutdownStarting(DispatcherQueue sender, DispatcherQueueShutdownStartingEventArgs e)
        {
            ((IDisposable)this).Dispose();
        }

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

        private void InitializeServices(ServiceCollection services)
        {
            services.AddSingleton<IMainPageNavigationService, MainPageNavigationService>()
                    .AddSingleton<IMainWindowNavigationService, MainWindowNavigationService>();

            services.AddSingleton<IAppLifetime>(this);

            services.AddSingleton<IWindowManager, WindowManager>()
                    .AddSingleton<ICultureService, CultureService>()
                    .AddSingleton<IBackdropManager, BackdropManager>()
                    .AddSingleton<ISettingsService, SettingsService>()
                    .AddSingleton<IPackageInformation, PackageInformation>();

            services.AddTransient<MainPageViewModel>()
                    .AddTransient<MainWindowViewModel>()
                    .AddTransient<RenamerWindowViewModel>();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            new MainWindow(_provider.GetRequiredService<MainWindowViewModel>()).Activate();
        }

        public void Restart()
        {
            ((IDisposable)this).Dispose();
            AppInstance.Restart(arguments: string.Empty);
        }

        void IDisposable.Dispose()
        {
            _provider.Dispose();
        }
    }
}
