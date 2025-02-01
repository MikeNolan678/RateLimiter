using Microsoft.AspNetCore.Http;
using RateLimiter.Configuration;

namespace RateLimiter.Middleware;

internal class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    
    private readonly RateLimitOptions _options;
    
    public RateLimitMiddleware(RequestDelegate next, RateLimitOptions options)
    {
        _next = next;
        _options = options;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        // TODO: Apply rate limiting logic here...
        
        // Call the next delegate/middleware in the pipeline.
        await _next(context);
    }
}