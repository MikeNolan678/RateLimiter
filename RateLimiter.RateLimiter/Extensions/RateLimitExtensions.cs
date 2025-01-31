using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Configuration;
using RateLimiter.Middleware;

namespace RateLimiter.Extensions;

/// <summary>Rate Limiter extensions.</summary>
public static class RateLimitExtensions
{
    /// <summary>Add an In Memory Rate Limiter to the IServiceCollection.</summary>
    public static IServiceCollection AddInMemoryRateLimiter(this IServiceCollection services)
    {
        services.AddMemoryCache();
        
        return services;
    }
    
    /// <summary>Add a Distributed Cache Rate Limiter to the IServiceCollection.</summary>
    public static IServiceCollection AddDistributedRateLimiter(this IServiceCollection services)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>Register the RateLimiter middleware and define the RateLimitOptions to be used.</summary>
    /// <param name="app">The IApplicationBuilder instance.</param>
    /// <param name="configureOptions">The Rate Limiter configuration.</param>
    public static IApplicationBuilder UseRateLimiter(this IApplicationBuilder app, Action<RateLimitOptions> configureOptions)
    {
        var options = new RateLimitOptions();
        
        configureOptions(options);
        
        app.UseMiddleware<RateLimitMiddleware>(options);
        return app;
    }
}