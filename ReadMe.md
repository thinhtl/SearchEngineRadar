

# Search Engine Radar - C# .NET 8

This project automates SEO rank checking for sympli.com.au. It takes keywords and a URL, returning positions (up to 100) in search results for each keyword.

  

## Features:

  

- Checks **Google** and **Bing** ranking

- Limits request to search engine by caching the results.

  

## Tech Stack:

  

- C# .NET 8

  

## Future Improvements:

-  **Persistence**: Store search results in a database to track website rankings over time, enabling historical analysis and trend visualization.

-  **UI/UX Enhancements:** Create a user-friendly interface with charts, filtering, sorting, and potentially user management for a comprehensive search monitoring tool.

  

## Code Structure:

**SearchRadar:** The SearchRadar class efficiently searches multiple search engines in parallel to find the ranking positions of a given website for a specific keyword, utilizing a cache to optimize performance and avoid blocking from Search Engine.

**SearchEngineBase:** SearchEngineBase is an abstract base class that provides a common structure and functionality for interacting with different search engines. It handles fetching search results, extracting URLs, building query URLs, and error logging, while allowing specific search engine implementations to define their unique URL patterns and query structures.

**GoogleSearchEngine:** Concrete implementation of SearchEngineBase for Google

**BingSearchEngine:** Concrete implementation of SearchEngineBase for Bing

**SearchEngineOptions:** Configuration options per search engine

  

## AppSettings Guide

This guide explains the configuration options available in the appsettings.json file for the application.

  

### SearchRadar:

  

**SearchDepth** *(integer)*: Defines the maximum depth (number of result) to explore within search results. Defaults to 100.

**CacheDurationSeconds** *(integer)*: Sets the duration (in seconds) to cache search results. This helps limit calls to search engines . Defaults to 3600 seconds (1 hour).

  

### SearchEngine:

  

**SearchResultPattern*** (string)*: Regular expression used to identify search result URLs within the retrieved HTML content.

**SearchEngineUrl *(string)*:** Base URL for constructing Search Engine queries.

**SearchQuery *(string)*:** URL template for building queries with {keyword} replaced by the actual keyword.

**PageQuery *(string)*:** The query string used to navigate to the next page of search results, {start} replaced by the page number (for pagination).

**PageSize** *(integer)*: Number of search results to retrieve per Google search request.

**RequestTimeoutMiliseconds** *(integer)***: Timeout (in milliseconds) for making HTTP requests to Google.

**MatchingTimeoutMiliseconds** *(integer):* **Timeout (in milliseconds) for identifying search results within the retrieved HTML content.

**DelayBetweenRequestsMiliseconds** *(integer)*: Delay (in milliseconds) to introduce between consecutive requests to Search Engine help avoid the search engine block the client.

**CustomHeader *(array of dictionary)*:** An optional array of custom HTTP headers to include in requests to Search Engine. This can be used to mimic a real browser for better compatibility. The **SearchEngineBase** class will randomly choose the header set from array.

  

## Adding a New Search Engine with SearchEngineBase

This guide explains how to create a new search engine implementation inheriting from the **SearchEngineBase** class.

### Developing a New Search Engine:

This guide will walk you through the process of creating a new search engine using the provided SearchEngineBase class as a foundation.

### Implement Abstract Property:

-  **Name**: Override the Name property to provide a unique name for your search engine. The Name property is also using for retrieve settings for the Search Engine using Named Options Pattern. By default, we use name of the Search Engine Class for the value.

  

### Customize Behavior (Optional):

- **BuildQueryUrl**: Construct the specific query URL for your search engine based on the keyword and starting index for pagination.

- **ExtractUrls**: Implement the logic to extract URLs from the HTML content returned by your search engine. By default, the **SearchEngineBase** use Regex to extract the Urls

- **QueryAsync** : Override this method if you need to modify the overall search behavior.

- **FetchHtmlAsync**; Override this method if your search engine requires specific HTTP request handling.

  

### Configuration and Dependency Injection

Create an extension method similar to **GoogleSearchEngineExtensions** to configure your search engine:

  

public static IServiceCollection AddBingSearchEngine(this IServiceCollection services, IConfiguration configuration)

    {
    
	    // Validate configuration argument
	    
	    ArgumentNullException.ThrowIfNull(configuration);
	    
	    string optionName = nameof(BingSearchEngine);
	    
	    // Configure SearchEngineOptions for Bing using a dedicated section in configuration
	    
	    services.Configure<SearchEngineOptions>(optionName, configuration.GetSection(optionName));
	    
	    // Register BingSearchEngine as the concrete implementation for ISearchEngine
	    
	    services.AddHttpClient<ISearchEngine, BingSearchEngine>();
	    
	    return services;
    
    }
