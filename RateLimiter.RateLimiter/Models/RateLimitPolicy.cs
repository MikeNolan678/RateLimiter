using RateLimiter.Configuration;

namespace RateLimiter.Models;

/// <summary>Represents a single rate limit policy.</summary>
public sealed class RateLimitPolicy
{
    /// <summary>
    /// The rate limit for the policy.
    /// </summary>
    public RateLimit RateLimit { get; set; } = new();
    
    /// <summary>
    /// The type of policy, indicating how the rate limit should be applied.
    /// </summary>
    public PolicyType PolicyType { get; set; }
    
    /// <summary>
    /// The client ID options for the rate limit policy. This is used to identify the client.
    /// </summary>
    public ClientIdOptions? ClientIdOptions { get; set; }
}