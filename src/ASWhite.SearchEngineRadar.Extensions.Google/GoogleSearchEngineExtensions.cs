using ASWhite.SearchEngineRadar.Core.Interfaces;
using ASWhite.SearchEngineRadar.Extensions.Google;
using ASWhite.SearchEngineRadar.Infrastructure.SearchEngineRadars;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class GoogleSearchEngineExtensions
{
    /// <summary>
    /// Adds the Google search engine functionality to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The IConfiguration instance used to access configuration settings.</param>
    /// <returns>The IServiceCollection instance with Google search engine services added.</returns>
    public static IServiceCollection AddGoogleSearchEngine(this IServiceCollection services, IConfiguration configuration)
    {
        // Validate configuration argument
        ArgumentNullException.ThrowIfNull(configuration);

        string optionName = nameof(GoogleSearchEngine);

        // Configure SearchEngineOptions for Google using a dedicated section in configuration
        services.Configure<SearchEngineOptions>(optionName, configuration.GetSection(optionName));

        // Register GoogleSearchEngine as the concrete implementation for ISearchEngine
        services.AddHttpClient<ISearchEngine, GoogleSearchEngine>();

        return services;
    }
}