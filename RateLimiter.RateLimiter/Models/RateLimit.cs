using RateLimiter.Configuration;

namespace RateLimiter.Models;

/// <summary>
/// Represents a single rate limits configuration.
/// </summary>
public class RateLimit
{
    /// <summary>
    /// The time window for the rate limit policy. Once the window has expired, the number of requests will reset.
    /// </summary>
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(Constants.DefaultWindowInMinutes);
    
    /// <summary>
    /// The number of requests allowed within the window.
    /// </summary>
    public int Limit { get; set; } = Constants.DefaultRequestLimit;
}