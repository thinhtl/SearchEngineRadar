using ASWhite.SearchEngineRadar.Core;
using ASWhite.SearchEngineRadar.Core.Interfaces;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ASWhite.SearchEngineRadar.CoreTests;

public class SearchRadarTests
{
    private readonly Mock<IEnumerable<ISearchEngine>> _mockSearchEngines;
    private readonly Mock<IResultlCache> _mockResultlCache;
    private readonly Mock<IOptions<SearchRadarOptions>> _mockOptions;
    private readonly SearchRadarOptions _options;
    private readonly SearchRadar _searchRadar;

    public SearchRadarTests()
    {
        _mockSearchEngines = new Mock<IEnumerable<ISearchEngine>>();
        _mockResultlCache = new Mock<IResultlCache>();
        _mockOptions = new Mock<IOptions<SearchRadarOptions>>();

        _options = new SearchRadarOptions
        {
            SearchDepth = 10,
            CacheDurationSeconds = 60
        };

        _mockOptions.Setup(o => o.Value).Returns(_options);

        var searchEngines = new List<ISearchEngine>
    {
        new Mock<ISearchEngine>().Object,
        new Mock<ISearchEngine>().Object
    };

        _mockSearchEngines.Setup(m => m.GetEnumerator()).Returns(searchEngines.GetEnumerator());

        _searchRadar = new SearchRadar(searchEngines, _mockResultlCache.Object, _mockOptions.Object);
    }

    [Fact]
    public async Task ScanAsync_ShouldReturnResults_WhenValidInputsAreGiven()
    {
        // Arrange
        Uri site = new ("https://example.com");
        string keyword = "test";

        var searchResults1 = new[] { "http://example.com/", "http://other.com/page2" };
        var searchResults2 = new[] { "http://example.com/page3", "https://example.com" };

        var mockSearchEngine1 = new Mock<ISearchEngine>();
        mockSearchEngine1.Setup(se => se.QueryAsync(keyword, _options.SearchDepth)).ReturnsAsync(searchResults1);
        mockSearchEngine1.Setup(se => se.Name).Returns("Engine1");

        var mockSearchEngine2 = new Mock<ISearchEngine>();
        mockSearchEngine2.Setup(se => se.QueryAsync(keyword, _options.SearchDepth)).ReturnsAsync(searchResults2);
        mockSearchEngine2.Setup(se => se.Name).Returns("Engine2");

        var searchEngines = new List<ISearchEngine> { mockSearchEngine1.Object, mockSearchEngine2.Object };

        var searchRadar = new SearchRadar(searchEngines, _mockResultlCache.Object, _mockOptions.Object);

        // Act
        var results = await searchRadar.ScanAsync(keyword, site);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(2, results.Count());

        var result1 = results.First(r => r.SearchEngineName == "Engine1");
        var result2 = results.First(r => r.SearchEngineName == "Engine2");

        Assert.Contains(1, result1.Indices); // "http://example.com/" should be included
        Assert.Contains(2, result2.Indices); // "https://example.com" should be included
    }

    [Fact]
    public async Task ScanAsync_ShouldUseCache_WhenResultsAreCached()
    {
        // Arrange
        Uri site = new("https://example.com");
        string keyword = "test";
        string cacheKey1 = $"{keyword}_Engine1";
        string cacheKey2 = $"{keyword}_Engine2";

        var cachedSearchResult1 = new List<string> { "http://example1.com", "http://example.com/" };
        var cachedSearchResult2 = new List<string> { "https://example.com", "http://example2.com?afs=asdg" };

        var cachedResult1 = new List<int> { 2 };
        var cachedResult2 = new List<int> { 1 };

        _mockResultlCache.Setup(c => c.GetCachedResult(cacheKey1)).Returns(cachedSearchResult1);
        _mockResultlCache.Setup(c => c.GetCachedResult(cacheKey2)).Returns(cachedSearchResult2);

        var mockSearchEngine1 = new Mock<ISearchEngine>();
        mockSearchEngine1.Setup(se => se.Name).Returns("Engine1");

        var mockSearchEngine2 = new Mock<ISearchEngine>();
        mockSearchEngine2.Setup(se => se.Name).Returns("Engine2");

        var searchEngines = new List<ISearchEngine> { mockSearchEngine1.Object, mockSearchEngine2.Object };

        var searchRadar = new SearchRadar(searchEngines, _mockResultlCache.Object, _mockOptions.Object);

        // Act
        var results = await searchRadar.ScanAsync(keyword, site);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(2, results.Count());

        var result1 = results.First(r => r.SearchEngineName == "Engine1");
        var result2 = results.First(r => r.SearchEngineName == "Engine2");

        Assert.Equal(cachedResult1, result1.Indices);
        Assert.Equal(cachedResult2, result2.Indices);

        // Ensure QueryAsync was never called
        mockSearchEngine1.Verify(se => se.QueryAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        mockSearchEngine2.Verify(se => se.QueryAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ScanAsync_ShouldCacheResults_WhenNotCached()
    {
        // Arrange
        Uri site = new("https://example.com");
        string keyword = "test";
        string cacheKey1 = $"{keyword}_Engine1";

        var searchResults = new[] { "http://example.com/", "http://other.com/page2" };

        var mockSearchEngine = new Mock<ISearchEngine>();
        mockSearchEngine.Setup(se => se.QueryAsync(keyword, _options.SearchDepth)).ReturnsAsync(searchResults);
        mockSearchEngine.Setup(se => se.Name).Returns("Engine1");

        var searchEngines = new List<ISearchEngine> { mockSearchEngine.Object };

        var searchRadar = new SearchRadar(searchEngines, _mockResultlCache.Object, _mockOptions.Object);

        // Act
        var results = await searchRadar.ScanAsync(keyword, site);

        // Assert
        Assert.NotNull(results);
        Assert.Single(results);

        var result = results.First();

        Assert.Contains(1, result.Indices);

        _mockResultlCache.Verify(c => c.SetCachedResult(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<TimeSpan>()), Times.AtLeastOnce());
    }
}