namespace ASWhite.SearchEngineRadar.Core;

/// <summary>
/// Represents the results of a search by a single search engine.
/// </summary>
public record SearchRadarResult
{
    /// <summary>
    /// The name of the search engine that performed the search.
    /// </summary>
    public string SearchEngineName { get; init; } = string.Empty;

    /// <summary>
    /// A collection of zero-based indices within the search results where the target website was found.
    /// </summary>
    public IEnumerable<int> Indices { get; init; } = Enumerable.Empty<int>();
}
