namespace RateLimiter.Configuration;

/// <summary>
/// Constants used throughout the Rate Limiter.
/// </summary>
public class Constants
{
    /// <summary>
    /// The default time window for the rate limit policy.
    /// </summary>
    public const int DefaultWindowInMinutes = 1;
    
    /// <summary>
    /// The default number of requests allowed within the window.
    /// </summary>
    public const int DefaultRequestLimit = 100;
}