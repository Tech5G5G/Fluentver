using Microsoft.Extensions.DependencyInjection;
using Fluver.Options;
using Fluver.Contracts;

namespace Fluver.Extensions;

public static class ServiceCollectionExtensions
{
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
