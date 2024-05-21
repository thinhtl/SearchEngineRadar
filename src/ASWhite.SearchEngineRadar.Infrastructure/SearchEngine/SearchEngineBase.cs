using System.Globalization;
using System.Text.RegularExpressions;
using ASWhite.SearchEngineRadar.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ASWhite.SearchEngineRadar.Infrastructure.Logging;

namespace ASWhite.SearchEngineRadar.Infrastructure.SearchEngineRadars;

public abstract class SearchEngineBase : ISearchEngine
{
    /// <summary>
    /// Logger instance for recording events during search operations.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Configuration options specific to the search engine implementation.
    /// </summary>
    protected SearchEngineOptions Options { get; init; }

    /// <summary>
    /// HttpClient instance used for making HTTP requests to the search engine.
    /// </summary>
    protected HttpClient HttpClient{ get; init; }

    /// <summary>
    /// The name of the search engine. This property must be implemented by derived classes.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Constructor for SearchEngineBase.
    /// </summary>
    /// <param name="httpClient">The HttpClient instance to use for making HTTP requests.</param>
    /// <param name="option">An IOptionsMonitor instance providing access to search engine specific configuration options.</param>
    /// <param name="logger">An ILogger instance for logging events.</param>
    /// <exception cref="ArgumentNullException">Throws ArgumentNullException if option or logger is null.</exception>
    protected SearchEngineBase(HttpClient httpClient, IOptionsMonitor<SearchEngineOptions> option, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(option);

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        HttpClient = httpClient;
        Options = option.Get(Name);

        ConfigureDefaultHttpClient(HttpClient, Options);
    }

    /// <summary>
    /// Asynchronously performs a search query for the specified keyword and retrieves a limited number of results.
    /// This method is virtual, allowing derived classes to potentially override specific behaviors.
    /// </summary>
    /// <param name="keyword">The keyword to use for the search.</param>
    /// <param name="take">The maximum number of search results to retrieve.</param>
    /// <returns>An asynchronous task that returns an array of strings representing the search results.</returns>
    public async virtual Task<string[]> QueryAsync(string keyword, int take)
    {
        string[] results = [];
        for (int start = 0; start < take; start += Options.PageSize)
        {
            if (start != 0)
            {
                // Introduce a delay between requests to avoid the search engine blocking the client
                await Task.Delay(Options.DelayBetweenRequestsMiliseconds).ConfigureAwait(false);
            }

            // Build the query URL using the keyword and current start index
            Uri queryUrl = BuildQueryUrl(keyword, start);

            // Log the query details
            _logger.LogQuery(queryUrl, keyword, Name, start);

            string htmlContent;

            try
            {
                // Fetch the HTML content from the search engine
                htmlContent = await FetchHtmlAsync(queryUrl).ConfigureAwait(false);
            }
            catch (HttpRequestException httpRequestException)
            {
                // Handle HttpRequestException and wrap it in a SearchEngineException
                SearchEngineException searchEngineException = new ("Failed to fetch search results from the search engine.");

                // Log the error with details
                _logger.LogSearchRequestError(keyword, Name, start, searchEngineException, httpRequestException);

                throw searchEngineException;
            }

            if (string.IsNullOrWhiteSpace(htmlContent))
            {
                // Handle empty search response
                _logger.LogEmptySearchResponeError(keyword, Name, start);

                throw new SearchEngineException("Empty search results from the search engine.");
            }

            // Extract URLs from the fetched HTML content
            string[] urls = ExtractUrls(htmlContent);

            if (urls.Length == 0)
            {
                // Handle failed URL extraction
                _logger.LogExtractingFailedError(keyword, Name, start, htmlContent);

                throw new SearchEngineException("Failed to extract search urls from the search engine.");
            }

            // Limit extracted URLs to the requested number (take) if necessary
            if (start + urls.Length > take)
            {
                urls = urls[..(take-start+1)];
            }

            // Combine extracted URLs with existing results
            results = [..results, ..urls];
        }

        return [.. results];
    }

    /// <summary>
    /// Abstract method that derived classes must implement to define their specific logic for extracting URLs from the fetched HTML content of a search page.
    /// This method is expected to return an array of strings representing the URLs found in the HTML content.
    /// </summary>
    /// <param name="htmlContent">The HTML content retrieved from the search engine.</param>
    /// <returns>An array of strings representing the extracted URLs.</returns>
    protected virtual string[] ExtractUrls(string htmlContent)
    {
        MatchCollection matches;

        try
        {
            // Attempt to match the search result pattern in the HTML content using the configured regular expression and timeout
            matches = Regex.Matches(htmlContent,
                Options.SearchResultPattern, 
                RegexOptions.None, 
                TimeSpan.FromMilliseconds(Options.MatchingTimeoutMiliseconds));
        }
        catch (RegexMatchTimeoutException)
        {
            // Handle timeout during pattern matching
            _logger.LogMatchingTimeoutError(Name, htmlContent);

            throw new SearchEngineException("Matching timeout.");
        }

        // Return the extracted URLs from the matched groups (group capture the whole value, group 1 captures the URL)
        return matches.Select(match => match.Groups[1].Value).ToArray();
    }

    /// <summary>
    /// Abstract method that derived classes must implement to define their specific logic for building the query URL based on the provided keyword and starting index.
    /// This method is expected to return a Uri object representing the constructed query URL.
    /// </summary>
    /// <param name="keywords">The keyword to use for the search.</param>
    /// <param name="start">The starting index for the search results (used for pagination).</param>
    /// <returns>A Uri object representing the constructed query URL.</returns>
    /// <exception cref="ArgumentException">Throws ArgumentException if the keyword is null, whitespace, or empty.</exception>
    protected virtual Uri BuildQueryUrl(string keywords, int start) 
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keywords);

        // Preprocess the keyword (trimming, lowercasing, replacing spaces with '+' for URL encoding)
        keywords = keywords.Trim()
            .ToLower(CultureInfo.CurrentCulture)
            .Replace(" ", "+", StringComparison.Ordinal);

        // Create a relative Uri object from the constructed query string
        string query = Options.SearchQuery.Replace("{keyword}", keywords, StringComparison.Ordinal);

        if (start > 0)
        {
            // Append the page query string if the start index is greater than 0
            string pageQuery = Options.PageQuery.Replace("{start}", start.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
            query = $"{query}&{pageQuery}";
        }

        return new Uri(query, UriKind.Relative);
    }

    /// <summary>
    /// Asynchronously fetches the HTML content from the specified Uri using the configured HttpClient.
    /// </summary>
    /// <param name="url">The Uri object representing the URL to fetch.</param>
    /// <returns>An asynchronous task that returns the fetched HTML content as a string.</returns>
    /// <exception cref="HttpRequestException">Throws HttpRequestException if there's an error during the HTTP request.</exception>
    protected async virtual Task<string> FetchHtmlAsync(Uri url)
    {
        HttpResponseMessage response = await HttpClient.GetAsync(url).ConfigureAwait(false);

        // Ensure successful response status code
        response.EnsureSuccessStatusCode();

        // Read the response content as a string
        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Configures the provided HttpClient instance with base URL, timeout, and custom headers based on the search engine options.
    /// </summary>
    /// <param name="httpClient">The HttpClient instance to configure.</param>
    /// <param name="options">The SearchEngineOptions object containing configuration settings.</param>
    private void ConfigureDefaultHttpClient(HttpClient httpClient, SearchEngineOptions options)
    {
        // Set the base URL for the search engine
        httpClient.BaseAddress = options.SearchEngineUrl;

        // Set the timeout for HTTP requests
        httpClient.Timeout = TimeSpan.FromMilliseconds(options.RequestTimeoutMiliseconds);

        // Add any custom headers specified in the options
        foreach (KeyValuePair<string, string> header in options.CustomHeader.First())
        {
            HttpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }
}