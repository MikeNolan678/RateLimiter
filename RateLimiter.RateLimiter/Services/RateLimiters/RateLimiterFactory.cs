using RateLimiter.Configuration;
using RateLimiter.Models;

namespace RateLimiter.Services.RateLimiters;

public class RateLimiterFactory : IRateLimiterFactory
{
    private readonly FixedWindowRateLimiter _fixedWindowRateLimiter;
    
    public RateLimiterFactory(FixedWindowRateLimiter fixedWindowRateLimiter)
    {
        _fixedWindowRateLimiter = fixedWindowRateLimiter;
    }
    
    /// <summary>
    /// Gets the rate limiter for the specified policy.
    /// </summary>
    public IRateLimiter GetRateLimiter(RateLimitPolicy policy)
    {
        return policy.RateLimitType switch
        {
            RateLimitType.FixedWindow => _fixedWindowRateLimiter,
            _ => throw new NotImplementedException()
        };
    }
}