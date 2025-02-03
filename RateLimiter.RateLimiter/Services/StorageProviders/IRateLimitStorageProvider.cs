namespace RateLimiter.Services.StorageProviders;

public interface IRateLimitStorageProvider
{
    /// <summary>
    /// Get or create a value for a given key in the cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="initialValue">The initial value.</param>
    /// <param name="expiration">The cache expiration time.</param>
    /// <typeparam name="T">The Type of object being saved to the cache.</typeparam>
    /// <returns>The value.</returns>
    Task<T?> GetOrCreateAsync<T>(string key, T? initialValue, TimeSpan expiration);
    
    /// <summary>
    /// Update the value for a key in the cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The updated value.</param>
    /// <typeparam name="T">The Type of object being saved to the cache.</typeparam> 
    /// <returns>The updated value.</returns>
    Task<T> UpdateAsync<T>(string key, T value);
}