using Microsoft.AspNetCore.Http;
using RateLimiter.Models;

namespace RateLimiter.Configuration;

public class RateLimitOptions
{
    /// <summary>
    /// The Rate Limit Policies.
    /// </summary>
    private readonly Dictionary<string, RateLimitPolicy> _policies = new();
    
    /// <summary>
    /// The Global Rate Limit Policy.
    /// </summary>
    public RateLimitPolicy? GlobalPolicy { get; private set; }
    
    /// <summary>
    /// Get the Rate Limit Policies.
    /// </summary>
    public IReadOnlyDictionary<string, RateLimitPolicy> GetPolicies => _policies;
    
    /// <summary>
    /// Called when a rate limit is exceeded, and overrides the default behavior.
    /// </summary>
    public Action<HttpContext>? OnLimitExceededOverride { get; private set; }
    
    /// <summary>
    /// Add a new policy to the rate limiter.
    /// </summary>
    public RateLimitOptions AddPolicy(string policyName, Action<RateLimitPolicyBuilder> configurePolicy)
    {
        RateLimitPolicyBuilder builder = new();
       
        configurePolicy(builder);
        
        _policies[policyName] = builder.Build();
        
        return this;
    }
    
    /// <summary>
    /// Set a global rate limit policy.
    /// </summary>
    public RateLimitOptions WithGlobalPolicy(Action<RateLimitPolicyBuilder> configurePolicy)
    {
        RateLimitPolicyBuilder builder = new();
        
        configurePolicy(builder);
        
        GlobalPolicy = builder.Build();
        
        return this;
    }
    
    /// <summary>
    /// Override the default behavior when a rate limit is exceeded.
    /// <br />
    /// <br />
    /// The default behavior is to return a 429 status code.
    /// </summary>
    public RateLimitOptions OnLimitExceeded(Action<HttpContext> onLimitExceeded)
    {
        OnLimitExceededOverride = onLimitExceeded;
        
        return this;
    }
}