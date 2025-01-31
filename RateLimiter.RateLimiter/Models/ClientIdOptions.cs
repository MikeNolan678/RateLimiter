namespace RateLimiter.Models;

/// <summary>
/// Represents the options for client identification.
/// </summary>
public class ClientIdOptions
{
    /// <summary>
    /// The request header to use for the rate limit policy. This is used to identify the client.
    /// </summary>
    public string? Header { get; set; }
}