using ASWhite.SearchEngineRadar.Infrastructure.SearchEngineRadars;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ASWhite.SearchEngineRadar.Extensions.Google;

/// <summary>
/// Concrete implementation of SearchEngineBase for Google search engine.
/// </summary>
/// <remarks>
/// Constructor for GoogleSearchEngine.
/// </remarks>
/// <param name="httpClient">The HttpClient instance used for making HTTP requests.</param>
/// <param name="option">An IOptionsMonitor instance providing access to search engine specific configuration options.</param>
/// <param name="logger">An ILogger instance for logging events.</param>
public class GoogleSearchEngine(HttpClient httpClient, IOptionsMonitor<SearchEngineOptions> option, ILogger<GoogleSearchEngine> logger) : SearchEngineBase(httpClient, option, logger)
{

    /// <summary>
    /// Overrides the Name property to return "GoogleSearchEngine".
    /// </summary>
    public override string Name => nameof(GoogleSearchEngine);
}
