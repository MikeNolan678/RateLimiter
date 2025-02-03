using RateLimiter.Models;

namespace RateLimiter.Configuration;

/// <summary>
/// Represents a builder for creating Rate Limit Policies.
/// </summary>
public class RateLimitPolicyBuilder
{
    private readonly RateLimitPolicy _policy = new();
    
    /// <summary>
    /// Adds a Fixed Window rate limit to the policy.
    /// </summary>
    /// <param name="numberOfRequestsLimit">The number of requests permitted within the specified window.</param>
    /// <param name="window">The window in which the permitted number of requests can be made.</param>
    public RateLimitPolicyBuilder FixedWindow(int numberOfRequestsLimit, TimeSpan window)
    {
        if (numberOfRequestsLimit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfRequestsLimit), "The number of requests limit must be greater than 0.");
        }
        
        if (window <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(window), "The window must be greater than 0.");
        }
        
        _policy.RateLimit = new RateLimit
        {
            Limit = numberOfRequestsLimit,
            Window = window
        };
        
        _policy.RateLimitType = RateLimitType.FixedWindow;
        
        return this;
    }
    
    /// <summary>
    /// Segregates the rate limit by IP Address of the request.
    /// The policy will be applied to the IP Address of the request.
    /// </summary>
    public RateLimitPolicyBuilder WithIpLimit()
    {
        _policy.PolicyType = PolicyType.IpAddress;
        
        return this;
    }
    
    /// <summary>
    /// Segregates the rate limit by Client ID, identified using the specified request header.
    /// <br />
    /// <br />
    /// For example, if the request header is "X-Client-Id", the policy will be applied to the value of the "X-Client-Id" header on the HttpRequest.
    /// </summary>
    /// <param name="requestHeader"></param>
    /// <returns></returns>
    public RateLimitPolicyBuilder WithClientIdLimit(string requestHeader)
    {
        if (string.IsNullOrWhiteSpace(requestHeader))
        {
            throw new ArgumentException("The request header must not be null or empty.", nameof(requestHeader));
        }
        
        _policy.PolicyType = PolicyType.ClientId;
        _policy.ClientId = new ClientId
        {
            Header = requestHeader,
        };
        
        return this;
    }
    
    /// <summary>
    /// Builds the rate limit policy.
    /// </summary>
    internal RateLimitPolicy Build()
    {
        return new RateLimitPolicy(_policy);
    }
}