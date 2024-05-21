using ASWhite.SearchEngineRadar.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace ASWhite.SearchEngineRadar.Core;

public class SearchRadar(IEnumerable<ISearchEngine> searchEngines, IResultlCache resultlCache, IOptions<SearchRadarOptions> options)
{
    /// <summary>
    /// Injected collection of search engine implementations.
    /// </summary>
    private readonly IEnumerable<ISearchEngine> _searchEngines = searchEngines ?? throw new ArgumentNullException(nameof(searchEngines));

    /// <summary>
    /// Injected implementation of the IResultlCache interface for caching search results.
    /// </summary>
    private readonly IResultlCache _resultlCache = resultlCache ?? throw new ArgumentNullException(nameof(resultlCache));

    /// <summary>
    /// Injected configuration options for SearchRadar.
    /// </summary>
    private readonly SearchRadarOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    /// Asynchronously scans the configured search engines for the specified keyword and site.
    /// </summary>
    /// <param name="webAddressUrl">The target website to search for.</param>
    /// <param name="keyword">The keyword to use for the search.</param>
    /// <returns>An asynchronous task that returns a collection of SearchRadarResult objects containing search engine names and their corresponding search result indices.</returns>
    public async Task<IEnumerable<SearchRadarResult>> ScanAsync(string keyword, Uri webAddressUrl)
    {
        // Create a collection of tasks to scan each search engine asynchronously
        IEnumerable<Task<SearchRadarResult>> scanTasks = _searchEngines.Select(searchEngine => ScanAsync(searchEngine, webAddressUrl, keyword));

        // Wait for all scan tasks to complete
        await Task.WhenAll(scanTasks).ConfigureAwait(false);

        // Return the results from each completed scan task
        return scanTasks.Select(task => task.Result);
    }

    /// <summary>
    /// Asynchronously scans a single search engine for the specified keyword and site.
    /// </summary>
    /// <param name="searchEngine">The search engine to use for the scan.</param>
    /// <param name="webAddressUrl">The target website to search for.</param>
    /// <param name="keyword">The keyword to use for the search.</param>
    /// <returns>An asynchronous task that returns a SearchRadarResult object containing the search engine name and its corresponding search result indices.</returns>
    private async Task<SearchRadarResult> ScanAsync(ISearchEngine searchEngine, Uri webAddressUrl, string keyword)
    {
        // Generate a cache key based on the keyword and search engine name
        string cacheKey = $"{keyword}_{searchEngine.Name}";

        // Check if the search result is already cached
        IEnumerable<string> searchResults = _resultlCache.GetCachedResult(cacheKey);

        if (! searchResults.Any())
        {
            // Perform the search query using the search engine
            searchResults = await searchEngine.QueryAsync(keyword, _options.SearchDepth).ConfigureAwait(false);

            // Cache the search results for the specified duration
            _resultlCache.SetCachedResult(cacheKey, searchResults, TimeSpan.FromSeconds(_options.CacheDurationSeconds));
        }
       
        // Find indices where the search results contain the target site
        IEnumerable<int> siteIndices = searchResults.Select((url, index) => new { Uri = new Uri(url), index })
                                                    .Where(result => CompareUrl(webAddressUrl, result.Uri))
                                                    .Select(result => result.index + 1);               

        // Return the search results for the specific search engine
        return new SearchRadarResult { SearchEngineName = searchEngine.Name, Indices = siteIndices };
    }

    private static bool CompareUrl(Uri url1, Uri url2)
    {
        return $"{url1.DnsSafeHost}{url1.PathAndQuery}".Equals($"{url2.DnsSafeHost}{url2.PathAndQuery}", StringComparison.OrdinalIgnoreCase);
    }
}
