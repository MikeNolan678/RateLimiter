using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Configuration;
using RateLimiter.Middleware;
using RateLimiter.Services.RateLimiters;
using RateLimiter.Services.StorageProviders;
using RateLimiter.Services.StorageProviders.InMemory;

namespace RateLimiter.Extensions;

/// <summary>Rate Limiter extensions.</summary>
public static class RateLimitExtensions
{
    /// <summary>Add an In Memory Rate Limiter to the IServiceCollection.</summary>
    public static IServiceCollection AddInMemoryRateLimiter(this IServiceCollection services)
    {
        services.AddSingleton<IRateLimitMemoryCache, RateLimitMemoryCache>();
        services.AddSingleton<IRateLimitStorageProvider, InMemoryRateLimitStorageProvider>();
        services.AddSingleton<FixedWindowRateLimiter>();
        services.AddSingleton<IRateLimiterFactory, RateLimiterFactory>();
        
        return services;
    }
    
    /// <summary>Add a Distributed Cache Rate Limiter to the IServiceCollection.</summary>
    public static IServiceCollection AddDistributedRateLimiter(this IServiceCollection services)
    {
        // services.AddSingleton<RateLimitDistributedCache>();
        // services.AddSingleton<IRateLimitStorageProvider, DistributedRateLimitStorageProvider>();
        
        // return services;
        
        throw new NotImplementedException();
    }
    
    /// <summary>Register the InMemoryRateLimiter middleware and define the RateLimitOptions to be used.</summary>
    /// <param name="app">The IApplicationBuilder instance.</param>
    /// <param name="configureOptions">The Rate Limiter configuration.</param>
    public static IApplicationBuilder UseInMemoryRateLimiter(this IApplicationBuilder app, Action<RateLimitOptions> configureOptions)
    {
        var options = new RateLimitOptions();
        
        configureOptions(options);
        
        app.UseMiddleware<InMemoryRateLimitMiddleware>(options);
        return app;
    }
    
    /// <summary>Register the DistributedRateLimiter middleware and define the RateLimitOptions to be used.</summary>
    /// <param name="app">The IApplicationBuilder instance.</param>
    /// <param name="configureOptions">The Rate Limiter configuration.</param>
    public static IApplicationBuilder UseDistributedRateLimiter(this IApplicationBuilder app, Action<RateLimitOptions> configureOptions)
    {
        // var options = new RateLimitOptions();
        
        // configureOptions(options);
        
        // app.UseMiddleware<DistributedRateLimitMiddleware>(options);
        // return app;
        
        throw new NotImplementedException();
    }
}