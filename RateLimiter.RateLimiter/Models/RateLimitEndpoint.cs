namespace RateLimiter.Models;

public sealed class RateLimitEndpoint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimitEndpoint"/> class.
    /// </summary>
    public RateLimitEndpoint() { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimitEndpoint"/> class.
    /// </summary>
    /// <param name="endpoint">The endpoint to copy to the new instance.</param>
    public RateLimitEndpoint(RateLimitEndpoint endpoint)
    {
        HttpMethod = endpoint.HttpMethod;
        Path = endpoint.Path;
        Policies = new List<string>(endpoint.Policies);
        
    }
    
    /// <summary>
    /// The path for the endpoint.
    /// <br />
    /// This defaults to <c>"/"</c>.
    /// </summary>
    public string Path { get; set; } = "/";
    
    /// <summary>
    /// The HTTP Method for the endpoint.
    /// <br />
    /// This defaults to <see cref="HttpMethod.Get"/>.
    /// </summary>
    public HttpMethod HttpMethod { get; set; } = HttpMethod.Get;
    
    /// <summary>
    /// The policies to apply to the endpoint.
    /// </summary>
    public List<string> Policies { get; set; } = [];
    
    /// <summary>
    /// Determines if the endpoint matches the specified HTTP Method and request path.
    /// </summary>
    public bool IsMatch(string? requestPath, HttpMethod httpMethod)
    {
        if (string.IsNullOrWhiteSpace(requestPath))
        {
            throw new ArgumentException("The request path cannot be null.", nameof(requestPath));
        }
        
        return HttpMethod == httpMethod &&
               (Path.Equals(requestPath, StringComparison.OrdinalIgnoreCase) ||
               (Path.EndsWith("/*") && requestPath.StartsWith(Path.TrimEnd('*'), StringComparison.OrdinalIgnoreCase)));
    }
}