using Microsoft.Extensions.Caching.Memory;

namespace RateLimiter.Services.StorageProviders.InMemory;

public interface IRateLimitMemoryCache
{
    /// <summary>
    /// The Memory Cache.
    /// </summary>
    MemoryCache Cache { get; }
}