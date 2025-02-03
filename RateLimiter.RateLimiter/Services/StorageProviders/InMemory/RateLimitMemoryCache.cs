using Microsoft.Extensions.Caching.Memory;

namespace RateLimiter.Services.StorageProviders.InMemory;

public class RateLimitMemoryCache : IRateLimitMemoryCache
{
    private const int DefaultCacheSizeLimit = 1_000_000;
    internal const int DefaultCacheEntrySize = 1;
    
    /// <summary>
    /// The Memory Cache.
    /// </summary>
    public MemoryCache Cache { get; } = new (
        new MemoryCacheOptions
        {
            SizeLimit = DefaultCacheSizeLimit,
        });
}