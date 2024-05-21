namespace ASWhite.SearchEngineRadar.Infrastructure.SearchEngineRadars;

/// <summary>
/// Represents configuration options for a search engine radar.
/// </summary>
public record SearchEngineOptions
{
    /// <summary>
    /// The base URL of the search engine used for constructing search queries.
    /// </summary>
    public required Uri SearchEngineUrl { get; init; }

    /// <summary>
    /// The keyword or search phrase to use for the search. Defaults to an empty string.
    /// </summary>
    public string SearchQuery { get; init; } = String.Empty;

    /// <summary>
    /// The query string used to navigate to the next page of search results. Defaults to an empty string.
    /// 
    public string PageQuery { get; init; } = String.Empty;

    /// <summary>
    /// A regular expression pattern used to identify search results within the retrieved HTML content. Defaults to an empty string.
    /// </summary>
    public string SearchResultPattern { get; init; } = String.Empty;

    /// <summary>
    /// The maximum depth (number of levels) to traverse during the search.
    /// </summary>
    public int SearchDepth { get; init; }

    /// <summary>
    /// The number of search results to retrieve per page (request).
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// The timeout in milliseconds for making HTTP requests to the search engine.
    /// </summary>
    public int RequestTimeoutMiliseconds { get; init; }

    /// <summary>
    /// The timeout in milliseconds for identifying search results within the retrieved HTML content.
    /// </summary>
    public int MatchingTimeoutMiliseconds { get; init; }

    /// <summary>
    /// The delay in milliseconds to introduce between consecutive requests to the search engine.
    /// </summary>
    public int DelayBetweenRequestsMiliseconds { get; init; }

    /// <summary>
    /// A collection of custom HTTP headers to include in requests to the search engine. Defaults to an empty list.
    /// </summary>
    public IEnumerable<Dictionary<string, string>> CustomHeader { get; init; } = []; // Use List<Dictionary<string, string>> for better initialization
}
