namespace ASWhite.SearchEngineRadar.Core.Interfaces;

/// <summary>
/// Interface defining methods for caching search results.
/// </summary>
public interface IResultlCache
{
    /// <summary>
    /// Retrieves cached search results for a given keyword.
    /// </summary>
    /// <param name="keyword">The keyword used for the search.</param>
    /// <returns>An enumerable collection of cached search results, or empty if no cache entry exists.</returns>
    IEnumerable<string> GetCachedResult(string keyword);

    /// <summary>
    /// Stores search results for a specific keyword and cache key with a specified duration.
    /// </summary>
    /// <param name="cacheKey">A unique key for the cached data.</param>
    /// <param name="results">A collection of search results.</param>
    /// <param name="offset">The duration for which the results should be cached.</param>
    void SetCachedResult(string cacheKey, IEnumerable<string> results, TimeSpan offset);
}
