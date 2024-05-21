using ASWhite.SearchEngineRadar.Core;
using ASWhite.SearchEngineRadar.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASWhite.SearchEngineRadar.Pages;

/// <summary>
/// Model for the Index page, handles searching for a keyword on a web address.
/// </summary>
public class IndexModel(SearchRadar searchRadar) : PageModel
{
    private readonly SearchRadar _searchRadar = searchRadar ?? throw new ArgumentNullException(nameof(searchRadar));

    /// <summary>
    /// Search results from the SearchRadar.
    /// </summary>
    public IEnumerable<SearchRadarResult> SearchResults { get; private set; } = [];

    /// <summary>
    /// Notification message to display to the user.
    /// </summary>
    public string Notification { get; private set; } = String.Empty;

    /// <summary>
    /// Keyword to search for, bound to a query parameter or form field.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? Keyword { get; set; } = "e-settlement";

    /// <summary>
    /// Web address to search on, bound to a query parameter or form field.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? WebAddress { get; set; } = "www.sympli.com.au";

    /// <summary>
    /// Handles the HTTP GET request for the Index page.
    /// </summary>
    public async Task OnGetAsync()
    {
        Notification = String.Empty;

        if (String.IsNullOrWhiteSpace(Keyword))
        {
            Notification = "Please enter a keyword.";
            return;
        }

        if (String.IsNullOrWhiteSpace(WebAddress))
        {
            Notification = "Please enter a web address.";
            return;
        }

        // Prepend "https://" if no scheme is present:
        string queryWebAddress = WebAddress;
        if (! WebAddress.StartsWith("http://", StringComparison.OrdinalIgnoreCase) 
            && ! WebAddress.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            queryWebAddress = "https://" + WebAddress;
        }

        Uri webAddressUrl;

        try
        {
            webAddressUrl = new(queryWebAddress); // Parse the web address into a Uri
        }
        catch (UriFormatException)
        {
            Notification = "Invalid web address.";
            return;
        }

        try
        {
            // Perform the search using the SearchRadar:
            SearchResults = await _searchRadar.ScanAsync(Keyword, webAddressUrl).ConfigureAwait(false);
        }
        catch (SearchEngineException exception)
        {
            Notification = $"An error occurred while scanning search engines: {exception.Message}";
        }
    }
}
