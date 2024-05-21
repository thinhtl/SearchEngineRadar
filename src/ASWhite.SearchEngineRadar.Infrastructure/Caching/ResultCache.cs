using ASWhite.SearchEngineRadar.Core.Interfaces; // Import interface for result cache
using Microsoft.Extensions.Caching.Memory;

namespace ASWhite.SearchEngineRadar.Infrastructure.Caching;

/// <summary>
/// Implements the IResultCache interface for caching search results using in-memory storage.
/// </summary>
public class ResultCache(IMemoryCache memoryCache) : IResultlCache
{
    /// <summary>
    /// The injected IMemoryCache instance used for storing cached search results.
    /// </summary>
    private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

    /// <summary>
    /// Retrieves cached search results for the specified keyword.
    /// </summary>
    /// <param name="keyword">The keyword used for the search.</param>
    /// <returns>An IEnumerable of integers representing the cached search result indices, or null if no cache entry is found.</returns>
    public IEnumerable<string> GetCachedResult(string keyword)
    {
        if (_memoryCache.TryGetValue(keyword, value: out var cacheEntry) && cacheEntry is IEnumerable<string> results)
        {
            return results;
        }

        return [];
    }

    /// <summary>
    /// Stores search results in the cache for the specified keyword with a absolute expiration.
    /// </summary>
    /// <param name="cacheKey">The keyword to use as the cache key.</param>
    /// <param name="results">An IEnumerable of string representing the search result to cache.</param>
    /// <param name="offset">A TimeSpan value representing the absolute expiration for the cache entry.</param>
    public void SetCachedResult(string cacheKey, IEnumerable<string> results, TimeSpan offset)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(offset);

        _memoryCache.Set(cacheKey, results, cacheEntryOptions);
    }
}
