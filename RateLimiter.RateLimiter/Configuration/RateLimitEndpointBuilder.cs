using RateLimiter.Models;

namespace RateLimiter.Configuration;

/// <summary>
/// Represents a builder for creating rate limit endpoints.
/// </summary>
public class RateLimitEndpointBuilder
{
    private readonly RateLimitEndpoint _endpoint = new();
    
    /// <summary>
    /// Configures the path for the endpoint. This defaults to <c>"/"</c>.
    /// <br /><br />
    /// A wildcard (*) can be used to match any path that ends with the specified path.
    /// The wildcard must be at the end of the path, after a forward slash (/).
    /// <br /><br />
    /// For example, <c>"/api/*"</c> will match <c>"/api"</c>, <c>"/api/v1"</c>, <c>"/api/v1/users"</c>, etc.
    /// </summary>
    public RateLimitEndpointBuilder ForPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("The path must not be null or empty.", nameof(path));
        }
        
        _endpoint.Path = path;
        
        return this;
    }
    
    /// <summary>
    /// Configures the HTTP Method for the endpoint. This defaults to <c>GET</c>.
    /// <br /><br />
    /// Any valid <see cref="HttpMethod"/> can be used.
    /// This includes <c>GET</c>, <c>POST</c>, <c>PUT</c>, <c>DELETE</c>, <c>OPTIONS</c>, <c>HEAD</c>, <c>PATCH</c>, <c>TRACE</c>, <c>CONNECT</c>.
    /// </summary>
    public RateLimitEndpointBuilder ForMethod(HttpMethod method)
    {
        _endpoint.HttpMethod = method;
        
        return this;
    }
    
    /// <summary>
    /// Adds a policy to the endpoint, with a given policy name (case-insensitive).
    /// <br />
    /// Multiple policies can be added to an endpoint.
    /// <br />
    /// If a policy with the specified name is not defined, then it will not be applied.
    /// <br />
    /// If the policy is already applied to the endpoint, it will not be added again.
    /// </summary>
    /// <param name="policyName">The name of the policy to apply.</param>
    public RateLimitEndpointBuilder WithPolicy(string policyName)
    {
        if (string.IsNullOrWhiteSpace(policyName))
        {
            throw new ArgumentException("The policy name must not be null or empty.", nameof(policyName));
        }
        
        if (_endpoint.Policies.Exists(p => p.Equals(policyName, StringComparison.OrdinalIgnoreCase)) == false)
        {
            _endpoint.Policies.Add(policyName);
        }
        
        return this;
    }
    
    /// <summary>
    /// Builds the rate limit endpoint.
    /// </summary>
    public RateLimitEndpoint Build()
    {
        return new RateLimitEndpoint(_endpoint);
    }
    
}