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
    /// The Rate Limit Endpoints.
    /// </summary>
    private readonly List<RateLimitEndpoint> _endpoints = [];
    
    /// <summary>
    /// The Global Rate Limit Policy.
    /// </summary>
    public RateLimitPolicy? GlobalPolicy { get; private set; }
    
    /// <summary>
    /// The Rate Limit Policies.
    /// </summary>
    public IReadOnlyDictionary<string, RateLimitPolicy> Policies => _policies;
    
    /// <summary>
    /// The Rate Limit Endpoints.
    /// </summary>
    public IReadOnlyList<RateLimitEndpoint> Endpoints  => _endpoints;
    
    /// <summary>
    /// Called when a rate limit is exceeded, and overrides the default behavior.
    /// </summary>
    public Action<HttpContext>? OnLimitExceededOverride { get; private set; }
    
    /// <summary>
    /// Add a new policy to the rate limiter. If the policy name already exists, it will be overwritten.
    /// </summary>
    public RateLimitOptions AddPolicy(string policyName, Action<RateLimitPolicyBuilder> configurePolicy)
    {
        if (string.IsNullOrWhiteSpace(policyName))
        {
            throw new ArgumentException("The policy name must not be null or empty.", nameof(policyName));
        }
        
        RateLimitPolicyBuilder builder = new();
       
        configurePolicy(builder);
        
        _policies[policyName] = builder.Build();
        
        return this;
    }
    
    /// <summary>
    /// Set a global rate limit policy.
    /// <br />
    /// A global policy will apply to all endpoints, as well as any endpoint specific policies.
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
    
    /// <summary>
    /// Configure the rate limiter endpoints. This is where the rate limit policies are applied to the endpoints.
    /// <br />
    /// If no endpoints are configured, the rate limiter will not apply any rate limits unless a global policy is configured.
    /// <br />
    /// A global policy will apply to all endpoints, as well as any endpoint specific policies.
    /// </summary>
    /// <param name="configureEndpoints"></param>
    /// <returns></returns>
    public RateLimitOptions ConfigureEndpoint(Action<RateLimitEndpointBuilder> configureEndpoints)
    {
        RateLimitEndpointBuilder builder = new();
        
        configureEndpoints(builder);
        
        _endpoints.Add(builder.Build());
        
        return this;
    }
}