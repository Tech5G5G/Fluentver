using Microsoft.Extensions.DependencyInjection;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Fluver.Options;
using Fluver.Strings;
using Fluver.Contracts;
using Fluver.Extensions;
using Fluver.UI.ViewModels;
using Fluver.Platform;
using Fluver.Platform.Interop;
using Fluver.Services;
using Fluver.Services.Navigation;

namespace Fluver
{
    public sealed partial class App : Application, IApplicationLifetime, IDisposable
    {
        public static new App Current => (App)Application.Current;

        public static IServiceProvider Services => Current.ServiceProvider;

        public IServiceProvider ServiceProvider => _provider;
        private readonly ServiceProvider _provider;

        private readonly IResources _resources;
        private readonly IWindowManager _manager;

        public App()
        {
            InitializeComponent();

            var queue = DispatcherQueue.GetForCurrentThread();
            queue.ShutdownStarting += OnShutdownStarting;
            queue.ShutdownCompleted += OnShutdownCompleted;

#if !DEBUG
            UnhandledException += OnUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
#endif

            ServiceCollection services = new();
            InitializeServices(services);
            _provider = services.BuildServiceProvider();

            // Initialize required singleton services
            _provider.GetRequiredService<ICultureService>(); // Updates current culture info
            _provider.GetRequiredService<IBackdropManager>(); // Updates window backdrops

            // Cache services for later use
            _resources = _provider.GetRequiredService<IResources>();
            _manager = _provider.GetRequiredService<IWindowManager>();
        }

        private void InitializeServices(ServiceCollection services)
        {
            services.AddSingleton<IApplicationLifetime>(this);

            services.AddSingleton<ICultureService, CultureService>()
                    .AddSingleton<IPackageInformation, PackageInformation>();

            services.AddUIServices()
                    .AddNavigationServices();

            services.AddFluverSetting(
                        FluverSetting.StartupPage,
                        defaultValue: FluverPage.Previous,
                        p => p == FluverPage.InsiderPage && !WindowsInfo.IsInsider ? FluverPage.AboutPage : p)
                    .AddFluverSetting(
                        FluverSetting.PersistedPage,
                        defaultValue: FluverPage.AboutPage,
                        p => p < FluverPage.AboutPage || p > (WindowsInfo.IsInsider ? FluverPage.InsiderPage : FluverPage.StoragePage)
                            ? FluverPage.AboutPage : p)
                    .AddFluverSetting(FluverSetting.Backdrop, defaultValue: BackdropType.Mica);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _manager.CreateWindow(_provider.GetRequiredService<MainWindowViewModel>()).Show();
        }

        #region Application Lifetime

        private bool _restart;

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

        #endregion

        #region Error Handling

        private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            ShowNotificationForException(e.Exception);
        }

        private void OnAppDomainUnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                ShowNotificationForException(exception);
            }
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            ShowNotificationForException(e.Exception.InnerException);
        }

        private void ShowNotificationForException(Exception exception)
        {
            AppNotificationManager.Default.Show(
                new AppNotificationBuilder().AddText(_resources.ExceptionThrown)
                                            .AddText(_resources.ExceptionType(exception.GetType().ToString()))
                                            .AddText(_resources.ExceptionMessageResult(exception.Message, exception.HResult))
                                            .AddButton(new(_resources.ExceptionFile)
                                            {
                                                InvokeUri = new("https://github.com/Tech5G5G/Fluentver/issues/new?template=BUG-REPORT.yml")
                                            })
                                            .BuildNotification());
        }

        #endregion

        void IDisposable.Dispose()
        {
            _provider.Dispose();
        }
    }
}
