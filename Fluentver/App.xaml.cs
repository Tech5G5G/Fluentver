using Microsoft.Extensions.DependencyInjection;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Fluver.Helpers;
using Fluver.Options;
using Fluver.Windows;
using Fluver.Extensions;
using Fluver.Navigation;
using Fluver.ViewModels;
using Fluver.Globalization;
using Fluver.ApplicationModel;
using Fluver.System.Interop;

namespace Fluver
{
    public sealed partial class App : Application, IApplicationLifetime, IDisposable
    {
        public static new App Current => (App)Application.Current;

        public static IServiceProvider Services => Current.ServiceProvider;

        public IServiceProvider ServiceProvider => _provider;
        private readonly ServiceProvider _provider;

        private readonly IWindowManager _manager;

        private bool _restart;

        public App()
        {
            InitializeComponent();

            var queue = DispatcherQueue.GetForCurrentThread();
            queue.ShutdownStarting += OnShutdownStarting;
            queue.ShutdownCompleted += OnShutdownCompleted;

#if !DEBUG
            UnhandledException += OnUnhandledException;
#endif

            _ = PInvoke.SetPreferredAppMode(PreferredAppMode.AllowDark);
            PInvoke.FlushMenuThemes();

            ServiceCollection services = new();
            InitializeServices(services);
            _provider = services.BuildServiceProvider();

            // Initialize required singleton services
            _provider.GetRequiredService<ICultureService>(); // Updates current culture info
            _provider.GetRequiredService<IBackdropManager>(); // Updates window backdrops

            _manager = _provider.GetRequiredService<IWindowManager>();
        }

        private void OnShutdownStarting(DispatcherQueue sender, DispatcherQueueShutdownStartingEventArgs e)
        {
            ((IDisposable)this).Dispose();
        }

        private void OnShutdownCompleted(DispatcherQueue sender, object e)
        {
            if (_restart)
            {
                AppInstance.Restart(arguments: string.Empty);
            }
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
            services.AddSingleton<IApplicationLifetime>(this);

            services.AddFluverSetting(
                        FluverSetting.StartupPage,
                        defaultValue: FluverPage.Previous,
                        p => p == FluverPage.InsiderPage && !VersionHelper.IsWindowsInsider ? FluverPage.AboutPage : p)
                    .AddFluverSetting(
                        FluverSetting.PersistedPage,
                        defaultValue: FluverPage.AboutPage,
                        p => p < FluverPage.AboutPage || p > (VersionHelper.IsWindowsInsider ? FluverPage.InsiderPage : FluverPage.StoragePage)
                            ? FluverPage.AboutPage : p)
                    .AddFluverSetting(FluverSetting.Backdrop, defaultValue: BackdropType.Mica);

            services.AddSingleton<IWindowManager, FluverWindowManager>()
                    .AddSingleton<ICultureService, CultureService>()
                    .AddSingleton<IBackdropManager, BackdropManager>()
                    .AddSingleton<IUISettingsService, UISettingsService>()
                    .AddSingleton<IPackageInformation, PackageInformation>();

            services.AddSingleton<IMainPageNavigationService, MainPageNavigationService>()
                    .AddSingleton<IMainWindowNavigationService, MainWindowNavigationService>();

            services.AddTransient<MainPageViewModel>()
                    .AddTransient<MainWindowViewModel>()
                    .AddTransient<SettingsPageViewModel>()
                    .AddTransient<RenamerWindowViewModel>();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _manager.CreateWindow(_provider.GetRequiredService<MainWindowViewModel>()).Show();
        }

        public void Restart()
        {
            _restart = true;
            Exit();
        }

        public new void Exit()
        {
            var windows = _manager.Windows;
            for (int i = 0; i < windows.Count; ++i)
            {
                windows[i].Close();
            }

            PInvoke.PostQuitMessage(nExitCode: 0);
        }

        void IApplicationLifetime.Exit()
        {
            Exit();
        }

        void IDisposable.Dispose()
        {
            _provider.Dispose();
        }
    }
}
