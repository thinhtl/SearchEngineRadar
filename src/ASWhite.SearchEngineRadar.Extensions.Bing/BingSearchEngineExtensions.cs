using ASWhite.SearchEngineRadar.Core.Interfaces;
using ASWhite.SearchEngineRadar.Extensions.Bing;
using ASWhite.SearchEngineRadar.Infrastructure.SearchEngineRadars;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class BingSearchEngineExtensions
{
    /// <summary>
    /// Adds the Bing search engine functionality to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The IConfiguration instance used to access configuration settings.</param>
    /// <returns>The IServiceCollection instance with Bing search engine services added.</returns>
    public static IServiceCollection AddBingSearchEngine(this IServiceCollection services, IConfiguration configuration)
    {
        // Validate configuration argument
        ArgumentNullException.ThrowIfNull(configuration);        

        string optionName = nameof(BingSearchEngine);

        // Configure SearchEngineOptions for Bing using a dedicated section in configuration
        services.Configure<SearchEngineOptions>(optionName, configuration.GetSection(optionName));

        // Register BingSearchEngine as the concrete implementation for ISearchEngine
        services.AddHttpClient<ISearchEngine, BingSearchEngine>();

        return services;
    }
}