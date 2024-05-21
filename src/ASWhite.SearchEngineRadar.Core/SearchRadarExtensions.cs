using ASWhite.SearchEngineRadar.Core;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding SearchRadar services to the dependency injection container.
/// </summary>
public static class SearchRadarExtensions
{
    /// <summary>
    /// Adds SearchRadar services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The IConfiguration instance to use for configuration.</param>
    /// <returns>An IServiceCollection object that has been modified to include SearchRadar services.</returns>
    public static IServiceCollection AddSearchRadar(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        // Configure SearchRadar options from appsettings.json or other configuration source
        services.Configure<SearchRadarOptions>(configuration.GetSection(nameof(SearchRadar)));

        // Register SearchRadar with transient lifetime (created per request)
        services.AddTransient<SearchRadar>();

        return services;
    }
}
