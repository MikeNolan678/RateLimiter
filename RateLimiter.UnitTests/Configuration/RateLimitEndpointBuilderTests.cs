using System.Diagnostics.CodeAnalysis;
using RateLimiter.Configuration;
using RateLimiter.Models;

namespace RateLimiter.UnitTests.Configuration;

[ExcludeFromCodeCoverage]
public class RateLimitEndpointBuilderTests : TestBase
{
    public static IEnumerable<object[]> HttpMethodTestData()
    {
        return new List<object[]>
        {
            new object[] { HttpMethod.Get },
            new object[] { HttpMethod.Post },
            new object[] { HttpMethod.Put },
            new object[] { HttpMethod.Delete },
            new object[] { HttpMethod.Patch },
            new object[] { HttpMethod.Head },
            new object[] { HttpMethod.Options },
            new object[] { HttpMethod.Trace },
            new object[] { HttpMethod.Connect }
        };
    }
    
    [Theory]
    [InlineData("/")]
    [InlineData("/api/*")]
    [InlineData("/api/v1/*")]
    [InlineData("/api/v1/users?param=value1")]
    public void ForPath_ShouldSetPath(string path)
    {
        RateLimitEndpointBuilder builder = new();
        
        builder.ForPath(path);
        
        var endpoint = builder.Build();
        
        Assert.Equal(path, endpoint.Path);
    }
    
    [Theory]
    [MemberData(nameof(InvalidStringTestData))]
    public void ForPath_ShouldThrowException_WhenPathIsInvalid(string path)
    {
        RateLimitEndpointBuilder builder = new();
        
        Assert.Throws<ArgumentException>(() => builder.ForPath(path));
    }
    
    [Theory]
    [MemberData(nameof(HttpMethodTestData))]
    public void ForMethod_ShouldSetMethod(HttpMethod method)
    {
        RateLimitEndpointBuilder builder = new();
        
        builder.ForMethod(method);
        
        var endpoint = builder.Build();
        
        Assert.Equal(method, endpoint.HttpMethod);
    }
    
    [Fact]
    public void DefaultValues_AreAppliedByDefault()
    {
        RateLimitEndpointBuilder builder = new();
        
        var endpoint = builder.Build();
        
        AssertPolicy("/", HttpMethod.Get, endpoint);
        Assert.Empty(endpoint.Policies);
    }
    
    [Fact]
    public void WithPolicy_ShouldAddPolicy()
    {
        RateLimitEndpointBuilder builder = new();
        
        builder.WithPolicy("TestPolicy");
        
        var endpoint = builder.Build();
        
        Assert.Single(endpoint.Policies);
        Assert.Contains("TestPolicy", endpoint.Policies);
    }
    
    [Fact]
    public void WithPolicy_ShouldAddMultiplePolicies_WhenNamesAreUnique()
    {
        RateLimitEndpointBuilder builder = new();
        
        builder.WithPolicy("TestPolicy1");
        builder.WithPolicy("TestPolicy2");
        
        var endpoint = builder.Build();
        
        Assert.Equal(2, endpoint.Policies.Count);
        Assert.Contains("TestPolicy1", endpoint.Policies);
        Assert.Contains("TestPolicy2", endpoint.Policies);
    }
    
    [Theory]
    [MemberData(nameof(InvalidStringTestData))]
    public void WithPolicy_ShouldThrowException_WhenPolicyNameInvalid(string policyName)
    {
        RateLimitEndpointBuilder builder = new();
        
        Assert.Throws<ArgumentException>(() => builder.WithPolicy(policyName));
    }
    
    // WithPolicy_ShouldNotAddPolicy_WhenPolicyNameIsDuplicate
    [Theory]
    [InlineData("TestPolicy", "TestPolicy")]
    [InlineData("TestPolicy", "testpolicy")]
    [InlineData("TestPolicy", "TESTPOLICY")]
    [InlineData("testpolicy123", "TestPolicy123")]
    [InlineData("/Test-policy123!", "/test-policy123!")]
    [InlineData("123", "123")]
    public void WithPolicy_ShouldNotAddPolicy_WhenPolicyNameIsDuplicate(string policyName1, string policyName2)
    {
        RateLimitEndpointBuilder builder = new();
        
        builder.WithPolicy(policyName1);
        builder.WithPolicy(policyName2);
        
        var endpoint = builder.Build();
        
        Assert.Single(endpoint.Policies);
        Assert.Contains(policyName1, endpoint.Policies);
    }
    
    [Fact]
    public void Build_ReturnsImmutablePolicy()
    {
        const string policyName = "TestPolicy";
        const string path = "/api/v1/*";
        
        RateLimitEndpointBuilder builder = new();
        
        builder.ForPath(path)
            .ForMethod(HttpMethod.Get)
            .WithPolicy(policyName);
        
        var endpoint = builder.Build();
        
        builder.ForPath("/api/v2/*")
            .ForMethod(HttpMethod.Post)
            .WithPolicy("TestPolicy2");
        
        
        Assert.Single(endpoint.Policies);
        Assert.Contains(policyName, endpoint.Policies);
        AssertPolicy(path, HttpMethod.Get, endpoint);
    }
    
    /// <summary>
    /// Compare the specified path and method to the generated endpoint.
    /// </summary>
    private void AssertPolicy(string path, HttpMethod method, RateLimitEndpoint endpoint)
    {
        Assert.Equal(path, endpoint.Path);
        Assert.Equal(method, endpoint.HttpMethod);
    }
}