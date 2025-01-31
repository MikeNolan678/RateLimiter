namespace RateLimiter.Configuration;

/// <summary>
/// The policy types available for rate limiting.
/// </summary>
public enum PolicyType
{
    /// <summary>
    /// Indicates that the rate limit it applied to an IP address.
    /// </summary>
    IpAddress,
    
    /// <summary>
    /// Indicates that the rate limit is applied to a client ID, which is identified using a specified request header.
    /// </summary>
    ClientId
}