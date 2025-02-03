using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace RateLimiter.Services.StorageProviders.InMemory;

public class InMemoryRateLimitStorageProvider : IRateLimitStorageProvider
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> Semaphores = [];
    
    private readonly MemoryCache _memoryCache;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryRateLimitStorageProvider"/> class.
    /// </summary>
    /// <param name="memoryCache">The RateLimitMemoryCache.</param>
    public InMemoryRateLimitStorageProvider(IRateLimitMemoryCache memoryCache)
    {
        _memoryCache = memoryCache.Cache;
    }
    
    /// <summary>
    /// Get or create a value for a given key in the cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="initialValue">The initial value.</param>
    /// <param name="expiration">The cache expiration time.</param>
    /// <returns>The value.</returns>
    public async Task<T?> GetOrCreateAsync<T>(string key, T? initialValue, TimeSpan expiration)
    {
        var semaphore = Semaphores.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        
        ArgumentNullException.ThrowIfNull(initialValue);
        
        await semaphore.WaitAsync();
        
        try
        {
            if (!_memoryCache.TryGetValue(key, out T? cachedResult))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(expiration)
                    .SetSize(RateLimitMemoryCache.DefaultCacheEntrySize);
                
                cachedResult = _memoryCache.Set(key, initialValue, cacheEntryOptions);
            }
            
            initialValue = cachedResult;
        }
        finally
        {
            semaphore.Release();
        }
        
        return initialValue;
    }
    
    /// <summary>
    /// Update the value for a key in the cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The updated value.</param>
    /// <returns>The updated value.</returns>
    public async Task<T> UpdateAsync<T>(string key, T value)
    {
        var semaphore = Semaphores.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        
        await semaphore.WaitAsync();
        
        try
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(RateLimitMemoryCache.DefaultCacheEntrySize);
            
            value = _memoryCache.Set(key, value, cacheEntryOptions);
        }
        finally
        {
            semaphore.Release();
        }
        
        return value;
    }
}