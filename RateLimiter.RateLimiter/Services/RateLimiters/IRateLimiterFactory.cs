using RateLimiter.Models;

namespace RateLimiter.Services.RateLimiters;

public interface IRateLimiterFactory
{
    /// <summary>
    /// Gets the rate limiter for the specified policy.
    /// </summary>
    IRateLimiter GetRateLimiter(RateLimitPolicy policy);
}