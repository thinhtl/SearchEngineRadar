using ASWhite.SearchEngineRadar.Core.Interfaces;
using ASWhite.SearchEngineRadar.Infrastructure.Caching;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for IServiceCollection to simplify adding ResultCache functionality.
/// </summary>
public static class ResultCacheExtensions
{
    /// <summary>
    /// Adds the necessary services to the IServiceCollection for using ResultCache for caching search results.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The IServiceCollection instance with ResultCache services added.</returns>
    public static IServiceCollection AddResultCache(this IServiceCollection services)
    {
        // Add the in-memory cache service (dependency for ResultCache)
        services.AddMemoryCache();

        // Register the ResultCache implementation as a singleton service
        services.AddSingleton<IResultlCache, ResultCache>();

        return services;
    }

}

