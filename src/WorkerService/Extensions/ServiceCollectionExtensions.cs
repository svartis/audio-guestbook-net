using Microsoft.Extensions.Options;

namespace AudioGuestbook.WorkerService.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Explicitly register the settings object by delegating to the IOptions object
    /// </summary>
    public static IServiceCollection AddAppSettings<T>(this IServiceCollection services, IConfiguration configuration) where T : class
    {
        services.Configure<T>(configuration)
            .AddSingleton(resolver => resolver.GetRequiredService<IOptions<T>>().Value);
        return services;
    }
}