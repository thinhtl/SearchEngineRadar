using ASWhite.SearchEngineRadar.Infrastructure.SearchEngineRadars;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ASWhite.SearchEngineRadar.Extensions.Bing; // Namespace for Bing search engine extension

/// <summary>
/// Concrete implementation of SearchEngineBase for Bing search engine.
/// </summary>
/// <remarks>
/// Constructor for BingSearchEngine.
/// </remarks>
/// <param name="httpClient">The HttpClient instance used for making HTTP requests.</param>
/// <param name="option">An IOptionsMonitor instance providing access to search engine specific configuration options.</param>
/// <param name="logger">An ILogger instance for logging events.</param>
public class BingSearchEngine(HttpClient httpClient, IOptionsMonitor<SearchEngineOptions> option, ILogger<BingSearchEngine> logger) : SearchEngineBase(httpClient, option, logger)
{

    /// <summary>
    /// Overrides the Name property to return "BingSearchEngine".
    /// </summary>
    public override string Name => nameof(BingSearchEngine);

    /// <summary>
    /// Building the query URL for Bing search Engine based on the provided keyword and starting index.
    /// This method is expected to return a Uri object representing the constructed query URL.
    /// </summary>
    /// <param name="keywords">The keyword to use for the search.</param>
    /// <param name="start">The starting index for the search results (used for pagination).</param>
    /// <returns>A Uri object representing the constructed query URL.</returns>
    /// <exception cref="ArgumentException">Throws ArgumentException if the keyword is null, whitespace, or empty.</exception>
    protected override Uri BuildQueryUrl(string keywords, int start)
    {
        if (start > 0)
            return base.BuildQueryUrl(keywords, start+1);

        return base.BuildQueryUrl(keywords, start);
    }
}
