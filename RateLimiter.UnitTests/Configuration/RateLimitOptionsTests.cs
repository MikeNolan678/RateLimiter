using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using RateLimiter.Configuration;

namespace RateLimiter.UnitTests.Configuration;

[ExcludeFromCodeCoverage]
public class RateLimitOptionsTests : TestBase
{
    private const int DefaultLimit = 10;
    private static readonly TimeSpan DefaultWindow = TimeSpan.FromMinutes(1);
    
    private readonly Action<RateLimitPolicyBuilder> _defaultFixedWindowPolicy = builder => builder.FixedWindow(DefaultLimit, DefaultWindow);
    
    [Fact]
    public void AddPolicy_ShouldAddPolicyToPolicies()
    {
        const string policyName = "TestPolicy";
        RateLimitOptions options = new();
        
        options.AddPolicy(policyName, _defaultFixedWindowPolicy);
        
        var policies = options.Policies;
        
        Assert.Single(policies);
        Assert.Contains(policyName, policies);
        
        var policy = policies[policyName];
        
        Assert.Equal(DefaultLimit, policy.RateLimit.Limit);
        Assert.Equal(DefaultWindow, policy.RateLimit.Window);
    }
    
    [Fact]
    public void AddPolicy_ShouldAddMultiplePolicies_WhenNameIsDifferent()
    {
        const string policyOneName = "PolicyOne";
        const string policyTwoName = "PolicyTwo";
        
        RateLimitOptions options = new();
        
        options.AddPolicy(policyOneName, _defaultFixedWindowPolicy);
        options.AddPolicy(policyTwoName, builder => builder.FixedWindow(20, TimeSpan.FromMinutes(2)));
        
        var policies = options.Policies;
        
        Assert.Collection(policies,
            policy1 =>
            {
                Assert.Equal(policyOneName, policy1.Key);
                Assert.Equal(DefaultLimit, policy1.Value.RateLimit.Limit);
                Assert.Equal(DefaultWindow, policy1.Value.RateLimit.Window);
            },
            policy2 =>
            {
                Assert.Equal(policyTwoName, policy2.Key);
                Assert.Equal(20, policy2.Value.RateLimit.Limit);
                Assert.Equal(TimeSpan.FromMinutes(2), policy2.Value.RateLimit.Window);
            });
    }
    
    [Fact]
    public void AddPolicy_ShouldOverwriteExistingPolicyWithSameName()
    {
        const string policyName = "TestPolicy";
        
        RateLimitOptions options = new();
        
        options.AddPolicy(policyName, _defaultFixedWindowPolicy);
        options.AddPolicy(policyName, builder => builder.FixedWindow(20, TimeSpan.FromMinutes(2)));
        
        var policies = options.Policies;
        
        Assert.Single(policies);
        Assert.Contains(policyName, policies);
        
        var policy = policies[policyName];
        
        Assert.Equal(20, policy.RateLimit.Limit);
        Assert.Equal(TimeSpan.FromMinutes(2), policy.RateLimit.Window);
    }
    
    [Theory]
    [MemberData(nameof(InvalidStringTestData))]
    public void AddPolicy_ShouldThrowException_WhenPolicyNameIsInvalid(string policyName)
    {
        RateLimitOptions options = new();
        
        Assert.Throws<ArgumentException>(() => options.AddPolicy(policyName, builder => builder.FixedWindow(DefaultLimit, DefaultWindow)));
    }
    
    [Fact]
    public void WithGlobalPolicy_ShouldSetGlobalPolicy()
    {
        RateLimitOptions options = new();
        
        options.WithGlobalPolicy(_defaultFixedWindowPolicy);
        
        var globalPolicy = options.GlobalPolicy;
        
        Assert.NotNull(globalPolicy);
        Assert.Equal(DefaultLimit, globalPolicy!.RateLimit.Limit);
        Assert.Equal(DefaultWindow, globalPolicy.RateLimit.Window);
    }
    
    [Fact]
    public void OnLimitExceeded_ShouldSetOnLimitExceededAction()
    {
        RateLimitOptions options = new();
        DefaultHttpContext httpContext = new();
        
        options.OnLimitExceeded(context =>
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        });
        
        options.OnLimitExceededOverride?.Invoke(httpContext);
        
        Assert.NotNull(options.OnLimitExceededOverride);
        Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);
    }
    
    [Theory]
    [InlineData("/api/*")]
    [InlineData("/api/v1/*")]
    [InlineData("/api/v1/users/*")]
    [InlineData("/api/test?param1=value1&param2=value2")]
    public void ConfigureEndpoint_ShouldAddEndpointToEndpoints(string path)
    {
        const string policyName = "TestPolicy";
        RateLimitOptions options = new();
        
        options.ConfigureEndpoint(endpoint =>
        {
            endpoint.ForMethod(HttpMethod.Get)
                .ForPath(path)
                .WithPolicy(policyName);
        });
        
        var endpoints = options.Endpoints;
        
        Assert.Single(endpoints);
        Assert.Collection(endpoints, endpoint =>
        {
            Assert.Equal(HttpMethod.Get, endpoint.HttpMethod);
            Assert.Equal(path, endpoint.Path);
            Assert.Contains(policyName, endpoint.Policies);
        });
    }
}