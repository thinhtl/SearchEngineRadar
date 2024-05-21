using System.Net;
using ASWhite.SearchEngineRadar.Core.Interfaces;
using ASWhite.SearchEngineRadar.Infrastructure.SearchEngineRadars;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace ASWhite.SearchEngineRadar.InfrastructureTests.SearchEngine;

public class SearchEngineBaseTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly Mock<IOptionsMonitor<SearchEngineOptions>> _mockOptionsMonitor;
    private readonly SearchEngineOptions _options;
    private readonly TestSearchEngine _searchEngine;
    private readonly TestLogger _testLogger;

    public SearchEngineBaseTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _mockOptionsMonitor = new Mock<IOptionsMonitor<SearchEngineOptions>>();
        _testLogger = new TestLogger();

        _options = new SearchEngineOptions
        {
            SearchEngineUrl = new Uri("https://example.com"),
            SearchQuery = "search?q={keyword}&start={start}",
            PageSize = 10,
            DelayBetweenRequestsMiliseconds = 100,
            RequestTimeoutMiliseconds = 1000,
            SearchResultPattern = "<a href=\"(.*?)\">",
            MatchingTimeoutMiliseconds = 5000,
            CustomHeader = [new Dictionary<string, string> { { "User-Agent", "TestAgent" } }]
        };

        _mockOptionsMonitor.Setup(o => o.Get(It.IsAny<string>())).Returns(_options);

        _searchEngine = new TestSearchEngine(_httpClient, _mockOptionsMonitor.Object, _testLogger);
    }

    [Fact]
    public async Task QueryAsync_ShouldReturnResults_WhenValidKeywordIsGiven()
    {
        // Arrange
        string keyword = "test";
        int take = 10;
        string htmlContent = "<a href=\"http://example.com/1\">Link</a><a href=\"http://example.com/2\">Link</a>";
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(htmlContent)
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var results = await _searchEngine.QueryAsync(keyword, take);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains("http://example.com/1", results);
        Assert.Contains("http://example.com/2", results);
    }

    [Fact]
    public async Task QueryAsync_ShouldLogErrorAndThrowException_WhenHttpRequestFails()
    {
        // Arrange
        string keyword = "test";
        int take = 10;
        var httpRequestException = new HttpRequestException();

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(httpRequestException);

        // Act & Assert
        await Assert.ThrowsAsync<SearchEngineException>(() => _searchEngine.QueryAsync(keyword, take));
        var logEntry = _testLogger.Logs.FirstOrDefault(log => log.Contains($"Request fail {keyword} from {_searchEngine.Name} start at 0"));
        Assert.NotNull(logEntry);
    }

    [Fact]
    public async Task QueryAsync_ShouldLogAndThrowException_WhenHtmlContentIsEmpty()
    {
        // Arrange
        string keyword = "test";
        int take = 10;
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(string.Empty)
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SearchEngineException>(() => _searchEngine.QueryAsync(keyword, take));
        var logEntry = _testLogger.Logs.FirstOrDefault(log => log.Contains($"Empty search content when query {keyword} from {_searchEngine.Name} start at 0."));
        Assert.NotNull(logEntry);
    }

    // Helper class to allow instantiation of the abstract SearchEngineBase
    private class TestSearchEngine(HttpClient httpClient, IOptionsMonitor<SearchEngineOptions> option, ILogger logger) : SearchEngineBase(httpClient, option, logger)
    {
        public override string Name => "TestSearchEngine";
    }

    // Helper class to capture log entries
    private class TestLogger : ILogger
    {
        public List<string> Logs { get; } = [];

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Logs.Add(formatter(state, exception));
        }
    }
}