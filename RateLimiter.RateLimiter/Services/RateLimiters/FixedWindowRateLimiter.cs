using Microsoft.AspNetCore.Http;
using RateLimiter.Models;
using RateLimiter.Services.StorageProviders;

namespace RateLimiter.Services.RateLimiters;

public class FixedWindowRateLimiter : IRateLimiter
{
    private readonly IRateLimitStorageProvider _storageProvider;
    
    public FixedWindowRateLimiter(IRateLimitStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }
    
    /// <summary>
    /// Applies the rate limit policy to the request.
    /// </summary>
    /// <param name="context">The HttpContext of the request.</param>
    /// <param name="options">The RateLimitPolicy to apply.</param>
    /// <returns>Returns true if the request has been rate limited, or false if the request is within the rate limit defined by the policy.</returns>
    public async Task<bool> IsRequestRateLimited(HttpContext context, RateLimitPolicy options)
    {
        long windowStartTimeAsUnixTime = GetWindowStartTimeAsUnixTime(options.RateLimit.Window);
        
        string? clientIdKey = GetClientIdKey( context, options);
        
        var key = $"rateLimitType:{options.RateLimitType.ToString()}-clientId:{clientIdKey}-window:{windowStartTimeAsUnixTime}";
        
        var expiration = GetCacheExpirationAsTimeSpan(windowStartTimeAsUnixTime, options.RateLimit.Window);
        
        int currentCount = await _storageProvider.GetOrCreateAsync(key, 1, expiration);
        
        if (currentCount <= options.RateLimit.Limit)
        {
            await _storageProvider.UpdateAsync(key, currentCount + 1);
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Gets a long which is a unix timestamp representing the start of the current window, based on the specified TimeSpan.
    /// <br />
    /// This is used to identify the window in which the rate limit is applied.
    /// <br />
    /// For example, if the window is 10 seconds, and the current time is 12:34:56UTC, the window start time would be 12:34:50UTC, represented as a unix timestamp.
    /// </summary>
    /// <param name="interval">The rate limit window TimeSpan.</param>
    /// <returns>A long which represents the start of the current window as a Unix timestamp.</returns>
    private static long GetWindowStartTimeAsUnixTime(TimeSpan interval)
    {
        long currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long intervalInSeconds = (long)interval.TotalSeconds;
        
        return currentUnixTime - (currentUnixTime % intervalInSeconds);
    }
    
    /// <summary>
    /// Gets the client ID key from the request headers, based on the specified RateLimitPolicy ClientId Header value.
    /// <br />
    /// This is used to identify the client for which the rate limit is applied.
    /// <br />
    /// For example, if the RateLimitPolicy ClientId Header is "X-Client-Id", the value of the "X-Client-Id" header on the HttpRequest will be returned.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    private string GetClientIdKey( HttpContext context, RateLimitPolicy options)
    {
        const string undefinedClientIdKey = "NotDefined";
        
        string? clientIdKey = null;
        
        if (options.ClientId?.Header is not null && context.Request.Headers.TryGetValue(options.ClientId.Header, out var headerValue))
        {
            clientIdKey = headerValue.FirstOrDefault();
        }
        
        return clientIdKey ?? undefinedClientIdKey;
    }
    
    /// <summary>
    /// Gets a TimeSpan representing the time until the cache entry expires.
    /// <br />
    /// This is used to set the expiration time for the cache entry, considering when the first request was made.
    /// Using a floored timespan to calculate the start of the window provides a consistent key for the cache,
    /// in order to retrieve the cache for other requests made within the window.
    /// However, it means additional calculations are required to determine the expiration time.
    /// </summary>
    /// <param name="windowStartTimeAsUnixTime"></param>
    /// <param name="window"></param>
    /// <returns></returns>
    private static TimeSpan GetCacheExpirationAsTimeSpan(long windowStartTimeAsUnixTime, TimeSpan window)
    {
        long currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long intervalInSeconds = (long)window.TotalSeconds;
        
        return TimeSpan.FromSeconds(windowStartTimeAsUnixTime + intervalInSeconds - currentUnixTime);
    }
}