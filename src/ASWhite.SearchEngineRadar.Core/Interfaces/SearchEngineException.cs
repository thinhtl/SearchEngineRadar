namespace ASWhite.SearchEngineRadar.Core.Interfaces;

/// <summary>
/// Exception class specific to search engine related errors.
/// </summary>
[Serializable]
public class SearchEngineException : Exception
{
    public SearchEngineException() { }

    public SearchEngineException(string message) : base(message) { }

    public SearchEngineException(string message, Exception inner) : base(message, inner) { }
}
