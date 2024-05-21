namespace ASWhite.SearchEngineRadar.Core;

/// <summary>
/// Configuration options for the SearchRadar class.
/// </summary>
public class SearchRadarOptions
{
    /// <summary>
    /// The maximum depth of search results to retrieve from each search engine.
    /// </summary>
    public int SearchDepth { get; init; }

    /// <summary>
    /// The duration (in seconds) to cache search results.
    /// </summary>
    public int CacheDurationSeconds { get; init; }
}
