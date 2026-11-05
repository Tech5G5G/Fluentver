using Microsoft.Extensions.DependencyInjection;
using Fluver.Options;
using Fluver.Strings;
using Fluver.Services;
using Fluver.Contracts;
using Fluver.UI.ViewModels;
using Fluver.Services.Navigation;

namespace Fluver.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUIServices(this IServiceCollection services)
    {
        services.AddSingleton<IResources, ResourcesProvider>()
                .AddSingleton<IWindowManager, FluverWindowManager>()
                .AddSingleton<IBackdropManager, BackdropManager>();

        services.AddTransient<MainPageViewModel>()
                .AddTransient<AboutPageViewModel>()
                .AddTransient<MainWindowViewModel>()
                .AddTransient<SettingsPageViewModel>()
                .AddTransient<RenamerWindowViewModel>();

        return services;
    }

    public static IServiceCollection AddNavigationServices(this IServiceCollection services)
    {
        services.AddSingleton<IMainPageNavigationService, MainPageNavigationService>()
                .AddSingleton<IMainWindowNavigationService, MainWindowNavigationService>();

        return services;
    }

    public static IServiceCollection AddFluverSetting<T>(
        this IServiceCollection services,
        FluverSetting setting,
        T defaultValue,
        Func<T, T> coercer = null)
    {
        services.AddKeyedSingleton<ISetting<T>>(setting, new Setting<T>(setting.ToString(), defaultValue, coercer));
        return services;
    }
}
