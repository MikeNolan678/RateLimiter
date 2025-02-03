using Microsoft.AspNetCore.Http;
using RateLimiter.Configuration;
using RateLimiter.Services.RateLimiters;

namespace RateLimiter.Middleware;

internal class InMemoryRateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimiterFactory _rateLimiterFactory;
    private readonly RateLimitOptions _options;
    
    public InMemoryRateLimitMiddleware(RequestDelegate next, IRateLimiterFactory rateLimiterFactory,  RateLimitOptions options)
    {
        _next = next;
        _rateLimiterFactory = rateLimiterFactory;
        _options = options;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        // TODO: Add logic for the global policy.
        if (_options.GlobalPolicy is not null)
        {
            // Process the global policy, reusing the same logic as the endpoint policies.
        }
        
        var currentPath = context.Request.Path;
        var currentMethod = HttpMethod.Parse(context.Request.Method);
        
        var isRateLimited = false;
        
        // TODO: Refactor below to make code more efficient, and reusable for use with the global policy.
        foreach (var endpoint in _options.Endpoints)
        {
            if (isRateLimited) break;
            if (!endpoint.IsMatch(currentPath, currentMethod)) continue;
            
            foreach (var policy in endpoint.Policies)
            {
                if (_options.Policies.TryGetValue(policy, out var rateLimitPolicy))
                {
                    var rateLimiter = _rateLimiterFactory.GetRateLimiter(rateLimitPolicy);
                    
                    isRateLimited = await rateLimiter.IsRequestRateLimited(context, rateLimitPolicy);
                    
                    if (isRateLimited)
                    {
                        HandleRateLimitedRequest(context);
                        
                        break;
                    }
                }
            };
        }
        
        if (!isRateLimited)
        {
            await _next(context);
        }
    }
    
    /// <summary>
    /// Handles the rate limit exceeded scenario.
    /// </summary>
    private void HandleRateLimitedRequest(HttpContext context)
    {
        if (_options.OnLimitExceededOverride is not null)
        {
            _options.OnLimitExceededOverride(context);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        }
    }
}