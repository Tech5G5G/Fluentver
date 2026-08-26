using Microsoft.Extensions.DependencyInjection;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
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

        private readonly IWindowManager _manager;

        public App()
        {
            InitializeComponent();
            DispatcherQueue.GetForCurrentThread().ShutdownStarting += OnShutdownStarting;

#if !DEBUG
            UnhandledException += OnUnhandledException;
#endif

            ServiceCollection services = new();
            InitializeServices(services);
            _provider = services.BuildServiceProvider();

            // Make sure culture info is set properly
            _provider.GetRequiredService<ICultureService>();
            _manager = _provider.GetRequiredService<IWindowManager>();
        }

        private void OnShutdownStarting(DispatcherQueue sender, DispatcherQueueShutdownStartingEventArgs e)
        {
            ((IDisposable)this).Dispose();
        }

        private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
            {
                e.Handled = true;

            // TODO: Localize
           AppNotificationManager.Default.Show(
               new AppNotificationBuilder().AddText("An exception was thrown.")
                    .AddText($"Type: {e.Exception.GetType()}")
                                           .AddText($"Message: {e.Message}{Environment.NewLine}" +
                             $"HResult: {e.Exception.HResult}")
                                           .AddButton(new("File an issue on GitHub")
                                           {
                                               InvokeUri = new("https://github.com/Tech5G5G/Fluentver/issues/new?template=BUG-REPORT.yml")
                                           })
                                           .BuildNotification());
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
                    .AddSingleton<IUISettingsService, UISettingsService>()
                    .AddSingleton<IPackageInformation, PackageInformation>();

            services.AddTransient<MainPageViewModel>()
                    .AddTransient<MainWindowViewModel>()
                    .AddTransient<RenamerWindowViewModel>();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _manager.CreateWindow(_provider.GetRequiredService<MainWindowViewModel>()).Show();
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
