namespace ASWhite.SearchEngineRadar.Core.Interfaces;

/// <summary>
/// Interface defining the basic functionality of a search engine.
/// </summary>
public interface ISearchEngine
{
    /// <summary>
    /// The name of the search engine.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Asynchronously performs a search query using the specified keyword and retrieves a limited number of results.
    /// </summary>
    /// <param name="keyword">The keyword to use for the search.</param>
    /// <param name="take">The maximum number of search results to retrieve.</param>
    /// <returns>An asynchronous task that returns an array of strings representing the search results.</returns>
    Task<string[]> QueryAsync(string keyword, int take);
}
