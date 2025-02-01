using System.Diagnostics.CodeAnalysis;
using RateLimiter.Models;

namespace RateLimiter.UnitTests.Configuration;

[ExcludeFromCodeCoverage]
public class RateLimitEndpointTests : TestBase
{
    public static IEnumerable<object[]> ValidEndpointTestData =>
        new List<object[]>
        {
            new object[] { "/api/test", HttpMethod.Get, "/api/test", HttpMethod.Get },
            new object[] { "/api/test", HttpMethod.Post, "/api/test", HttpMethod.Post },
            new object[] { "/api/test/*", HttpMethod.Get, "/api/test/123", HttpMethod.Get },
            new object[] { "/api/test/*", HttpMethod.Get, "/api/test/123/abc", HttpMethod.Get },
            new object[] { "/*", HttpMethod.Get, "/api/test/123", HttpMethod.Get },
            new object[] { "/*", HttpMethod.Get, "/", HttpMethod.Get },
            new object[] { "/", HttpMethod.Get, "/", HttpMethod.Get },
            new object[] { "/api/test", HttpMethod.Put, "/api/test", HttpMethod.Put },
            new object[] { "/api/test", HttpMethod.Delete, "/api/test", HttpMethod.Delete },
            new object[] { "/api/test/", HttpMethod.Get, "/api/test/", HttpMethod.Get },
            new object[] { "/api/TEST/", HttpMethod.Get, "/api/TEST/", HttpMethod.Get },
            new object[] { "/api/test/", HttpMethod.Get, "/api/Test/", HttpMethod.Get },
            new object[] { "/api/Test/", HttpMethod.Get, "/api/test/", HttpMethod.Get },
            new object[] { "/api/TEST/", HttpMethod.Get, "/api/test/", HttpMethod.Get },
            new object[] { "/*", HttpMethod.Get, "/?param=value", HttpMethod.Get },
            new object[] { "/*", HttpMethod.Get, "/api/test/123?param=value", HttpMethod.Get },
            new object[] { "/api/test-case/", HttpMethod.Get, "/api/test-case/", HttpMethod.Get },
            new object[] { "/api/test-case/?param=value", HttpMethod.Get, "/api/test-case/?param=value", HttpMethod.Get },
            new object[] { "/api/test/123?param=value", HttpMethod.Get, "/api/test/123?param=value", HttpMethod.Get },
            new object[] { "/api/test/123?param=value&?param2=value2", HttpMethod.Get, "/api/test/123?param=value&?param2=value2", HttpMethod.Get },
        };
    
    public static IEnumerable<object[]> InvalidEndpointTestData =>
        new List<object[]>
        {
            new object[] { "/api/test", HttpMethod.Get, "/api/test", HttpMethod.Post },
            new object[] { "/api/test", HttpMethod.Post, "/api/tests", HttpMethod.Post },
            new object[] { "/api/test/*", HttpMethod.Get, "/api/test", HttpMethod.Get },
            new object[] { "/api/test/*", HttpMethod.Get, "/api/test/123", HttpMethod.Post },
            new object[] { "/api/test*", HttpMethod.Get, "/api/test/123", HttpMethod.Get },
            new object[] { "/api/test-case/", HttpMethod.Get, "/api/test/", HttpMethod.Get },
            new object[] { "/api/test/", HttpMethod.Get, "/api/test-case/", HttpMethod.Get },
            new object[] { "/api/test/123?param=value", HttpMethod.Get, "/api/test/123", HttpMethod.Get },
            new object[] { "/api/test/123?param=value", HttpMethod.Get, "/api/test/123?param=value2", HttpMethod.Get },
            new object[] { "/api/test/123?param=value&?param2=value2", HttpMethod.Get, "/api/test/123?param=value", HttpMethod.Get },
        };
    
    [Theory]
    [MemberData(nameof(ValidEndpointTestData))]
    public void IsMatch_ReturnsTrue_WhenTheEndpointMatches(string configuredPath, HttpMethod configuredHttpMethod, string requestPath, HttpMethod requestHttpMethod)
    {
        RateLimitEndpoint endpoint = new()
        {
            Path = configuredPath,
            HttpMethod = configuredHttpMethod,
        };
        
        Assert.True(endpoint.IsMatch(requestPath, requestHttpMethod));
    }
    
    [Theory]
    [MemberData(nameof(InvalidEndpointTestData))]
    public void IsMatch_ReturnsFalse_WhenTheEndpointDoesNotMatch(string configuredPath, HttpMethod configuredHttpMethod, string requestPath, HttpMethod requestHttpMethod)
    {
        RateLimitEndpoint endpoint = new()
        {
            Path = configuredPath,
            HttpMethod = configuredHttpMethod,
        };
        
        Assert.False(endpoint.IsMatch(requestPath, requestHttpMethod));
    }
    
    [Theory]
    [MemberData(nameof(InvalidStringTestData))]
    public void IsMatch_ShouldThrowException_WhenRequestPathIsInvalidString(string? requestPath)
    {
        RateLimitEndpoint endpoint = new();
        
        Assert.Throws<ArgumentException>(() => endpoint.IsMatch(requestPath, HttpMethod.Get));
    }
}