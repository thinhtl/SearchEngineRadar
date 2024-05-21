using ASWhite.SearchEngineRadar.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace ASWhite.SearchEngineRadar.Infrastructure.Logging
{
    public static partial class Log
    {
        /// <summary>
        /// Logs a message for a search query initiated.
        /// </summary>
        /// <param name="logger">The ILogger instance used for logging.</param>
        /// <param name="queryUrl">The Uri object representing the constructed query URL.</param>
        /// <param name="keyword">The keyword used for the search.</param>
        /// <param name="searchEngine">The name of the search engine being queried.</param>
        /// <param name="start">The starting index for the search results (used for pagination).</param>
        [LoggerMessage(
            EventId = 0,
            Level = LogLevel.Warning,
            Message = "Query {Keyword} from {SearchEngine} start at {Start}. Query URL: {QueryUrl}")]
        public static partial void LogQuery(
            this ILogger logger, Uri queryUrl, string keyword, string searchEngine, int start);

        /// <summary>
        /// Logs a message for an error during a search request.
        /// </summary>
        /// <param name="logger">The ILogger instance used for logging.</param>
        /// <param name="keyword">The keyword used for the search.</param>
        /// <param name="searchEngine">The name of the search engine being queried.</param>
        /// <param name="start">The starting index for the search results (used for pagination).</param>
        /// <param name="searchEngineException">A SearchEngineException object containing details about the error.</param>
        /// <param name="httpRequestException">An HttpRequestException object containing details about the HTTP request error (if applicable).</param>
        [LoggerMessage(
            EventId = 1,
            Level = LogLevel.Error,
            Message = "Request fail {Keyword} from {SearchEngine} start at {Start}. {httpRequestException}")]
        public static partial void LogSearchRequestError(
            this ILogger logger, 
            string keyword, 
            string searchEngine, 
            int start, 
            SearchEngineException searchEngineException, 
            HttpRequestException httpRequestException);

        /// <summary>
        /// Logs a message for an empty search response.
        /// </summary>
        /// <param name="logger">The ILogger instance used for logging.</param>
        /// <param name="keyword">The keyword used for the search.</param>
        /// <param name="searchEngine">The name of the search engine being queried.</param>
        /// <param name="start">The starting index for the search results (used for pagination).</param>
        [LoggerMessage(
            EventId = 2,
            Level = LogLevel.Error,
            Message = "Empty search content when query {Keyword} from {SearchEngine} start at {Start}.")]
        public static partial void LogEmptySearchResponeError(
            this ILogger logger,
            string keyword,
            string searchEngine,
            int start);

        /// <summary>
        /// Logs a message for a failure to extract URLs from the search response.
        /// </summary>
        /// <param name="logger">The ILogger instance used for logging.</param>
        /// <param name="keyword">The keyword used for the search.</param>
        /// <param name="searchEngine">The name of the search engine being queried.</param>
        /// <param name="start">The starting index for the search results (used for pagination).</param>
        /// <param name="htmlContent">The HTML content retrieved from the search engine (might be useful for debugging).</param>
        [LoggerMessage(
            EventId = 3,
            Level = LogLevel.Error,
            Message = "Cannot extract urls when query {Keyword} from {SearchEngine} start at {Start}. {HtmlContent}")]
        public static partial void LogExtractingFailedError(
            this ILogger logger,
            string keyword,
            string searchEngine,
            int start,
            string htmlContent);

        /// <summary>
        /// Logs a message for a matching timeout error during a search.
        /// </summary>
        /// <param name="logger">The ILogger instance used for logging.</param>
        /// <param name="searchEngine">The name of the search engine being queried.</param>
        /// <param name="htmlContent">The HTML content retrieved from the search engine (might be useful for debugging).</param>
        [LoggerMessage(
            EventId = 4,
            Level = LogLevel.Error,
            Message = "Matching timeout while query from {SearchEngine}. {HtmlContent}")]
        public static partial void LogMatchingTimeoutError(
            this ILogger logger,
            string searchEngine,
            string htmlContent);
    }
}
