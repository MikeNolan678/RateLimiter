using Microsoft.AspNetCore.Http;
using RateLimiter.Models;

namespace RateLimiter.Services.RateLimiters;

public interface IRateLimiter
{
    /// <summary>
    /// Applies the rate limit policy to the request.
    /// </summary>
    /// <param name="context">The HttpContext of the request.</param>
    /// <param name="options">The RateLimitPolicy to apply.</param>
    /// <returns>Returns true if the request has been rate limited, or false if the request is within the rate limit defined by the policy.</returns>
    Task<bool> IsRequestRateLimited(HttpContext context, RateLimitPolicy options);
}