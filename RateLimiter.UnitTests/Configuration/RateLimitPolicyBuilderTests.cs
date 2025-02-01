using System.Diagnostics.CodeAnalysis;
using RateLimiter.Configuration;

namespace RateLimiter.UnitTests.Configuration;

[ExcludeFromCodeCoverage]
public class RateLimitPolicyBuilderTests : TestBase
{
    public static IEnumerable<object[]> FixedWindowValidTestData =>
        new List<object[]>
        {
            new object[] { 1, TimeSpan.FromSeconds(0.01) },
            new object[] { 10, TimeSpan.FromSeconds(10) },
            new object[] { 5, TimeSpan.FromMinutes(1) },
            new object[] { 100, TimeSpan.FromHours(1) },
            new object[] { 1000, TimeSpan.FromSeconds(0.5) },
        };
    
    public static IEnumerable<object?[]> FixedWindowInvalidTestData =>
        new List<object?[]>
        {
            new object[] { 1, TimeSpan.FromSeconds(0) },
            new object[] { 10, TimeSpan.FromSeconds(-10) },
            new object[] { 0, TimeSpan.FromSeconds(10) },
            new object[] { -10, TimeSpan.FromSeconds(10) },
            new object[] { 0, TimeSpan.FromSeconds(0) },
            new object[] { -10, TimeSpan.FromSeconds(-10) },
            new object?[] { null, null},
        };
    
    [Theory]
    [MemberData(nameof(FixedWindowValidTestData))]
    public void FixedWindow_ShouldSetRateLimit(int numberOfRequestsLimit, TimeSpan window)
    {
        RateLimitPolicyBuilder builder = new();
        
        builder.FixedWindow(numberOfRequestsLimit, window);
        
        var policy = builder.Build();
        
        Assert.Equal(numberOfRequestsLimit, policy.RateLimit.Limit);
        Assert.Equal(window, policy.RateLimit.Window);
    }
    
    [Theory]
    [MemberData(nameof(FixedWindowInvalidTestData))]
    public void FixedWindow_ShouldThrowException_WhenParametersNotValid(int numberOfRequestsLimit, TimeSpan window)
    {
        RateLimitPolicyBuilder builder = new();
        
        Assert.Throws<ArgumentOutOfRangeException>(() => builder.FixedWindow(numberOfRequestsLimit, window));
    }
    
    [Fact]
    public void WithIpLimit_SetsPolicyTypeToIpAddress()
    {
        RateLimitPolicyBuilder builder = new();
        
        builder.WithIpLimit();
        
        var policy = builder.Build();
        
        Assert.Equal(PolicyType.IpAddress, policy.PolicyType);
    }
    
    [Theory]
    [InlineData("X-Client-Id")]
    [InlineData("Authorization")]
    [InlineData("abcABC123")]
    [InlineData("123456")]
    public void WithClientIdLimit_SetsPolicyTypeToClientId_AndConfiguresClientId(string requestHeader)
    {
        RateLimitPolicyBuilder builder = new();
        
        builder.WithClientIdLimit(requestHeader);
        
        var policy = builder.Build();
        
        Assert.Equal(PolicyType.ClientId, policy.PolicyType);
        Assert.Equal(requestHeader, policy.ClientId?.Header);
    }
    
    [Theory]
    [MemberData(nameof(InvalidStringTestData))]
    public void WithClientIdLimit_ShouldThrowException_WhenRequestHeaderIsInvalid(string requestHeader)
    {
        RateLimitPolicyBuilder builder = new();
        
        Assert.Throws<ArgumentException>(() => builder.WithClientIdLimit(requestHeader));
    }
    
    [Fact]
    public void Build_ReturnsImmutablePolicy()
    {
        RateLimitPolicyBuilder builder = new();
        
        builder.FixedWindow(10, TimeSpan.FromMinutes(1));
        builder.WithClientIdLimit("X-Client-Id");
        
        var policy = builder.Build();
        
        builder.WithIpLimit();
        builder.FixedWindow(5, TimeSpan.FromSeconds(10));
        
        Assert.Equal(10, policy.RateLimit.Limit);
        Assert.Equal(TimeSpan.FromMinutes(1), policy.RateLimit.Window);
    }
}