using RateLimiter.Configuration;

namespace RateLimiter.Models;

/// <summary>Represents a single rate limit policy.</summary>
public sealed class RateLimitPolicy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimitPolicy"/> class.
    /// </summary>
    public RateLimitPolicy() { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimitPolicy"/> class.
    /// </summary>
    /// <param name="policy">The policy to copy to the new instance.</param>
    public RateLimitPolicy(RateLimitPolicy policy)
    {
        RateLimit = policy.RateLimit;
        PolicyType = policy.PolicyType;
        ClientId = policy.ClientId;
    }
    
    /// <summary>
    /// The rate limit for the policy.
    /// </summary>
    public RateLimit RateLimit { get; set; } = new();
    
    /// <summary>
    /// The type of policy, indicating how the rate limit should be applied.
    /// </summary>
    public PolicyType PolicyType { get; set; }
    
    /// <summary>
    /// The type of rate limit to apply.
    /// </summary>
    public RateLimitType RateLimitType { get; set; }
    
    /// <summary>
    /// The client ID for the rate limit policy. This is used to identify the client.
    /// </summary>
    public ClientId? ClientId { get; set; }
}