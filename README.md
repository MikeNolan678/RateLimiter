# 🚀 Rate Limiter for .NET

A flexible and configurable rate limiter for .NET applications, supporting a **Fixed Window** algorithm, with others coming soon.

## 📌 Features

- ✅ **Multiple Rate Limiting Strategies** – Supports **Fixed Window**, with **Token Bucket** and others planned.
- ✅ **NuGet** – (Coming Soon) - NuGet package for simple installation.
- ✅ **Configurable via Fluent API** – Configure with an intuitive syntax.
- ✅ **Chained endpoint specific policies** –  Define chained policies for specific endpoints for maximum flexibility, and apply multiple policies to one endpoint.
- ✅ **Global Rate Limiter** – Configure a **Global Rate Limiter** as a fallback, or for simple scenarios.
- ✅ **In-Memory & Distributed Caching** – Choose between **In Memory** and **Distributed** rate limiters (utilising In-Memory Cache or Redis) to accommodate distributed systems.



## ⚡ Quick Start
### 1️⃣ Register Rate Limiting Services
Add the required services in Program.cs:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInMemoryRateLimiter();

var app = builder.Build();
```

### 2️⃣ Define Rate Limit Policies and Endpoint configuration
Configure rate limit policies and how they apply to endpoints in your application:

```csharp
app.UseInMemoryRateLimiter(options =>
        {
            options.AddPolicy("ApiKey",
                policy =>
                {
                    policy
                        .FixedWindow(2, TimeSpan.FromSeconds(10))
                        .WithClientIdLimit("X-Api-Key");
                });
            
            options.ConfigureEndpoint(endpoint =>
            {
                endpoint.ForMethod(HttpMethod.Get)
                    .ForPath("/WeatherForecast")
                    .WithPolicy("ClientId")
                    .WithPolicy("ApiKey");
            });
            
        });
```

## 📝 API
Fluent API allows simple configuration of the rate limiter.
Configuration can be chained to provide maximum flexibility. See below for some use case examples. 

|Method |	Description|
|---|---|
|AddInMemoryRateLimiter()| Adds the services required for an In-Memory Rate Limiter.|
|UseInMemoryRateLimiter()| Adds the rate limiter middleware, with the specified configuration. |
|AddPolicy(policyName, configurePolicy) | Adds a named policy with the specified configuration.|
|FixedWindow(limit, window)	| Defines a fixed window rate limit, with the specified limit and window.|
|WithClientIdLimit(headerName)	| Applies rate limiting per client ID from a specified request header.|
|WithIpLimit()	| Applies rate limiting per IP address.|
|ForMethod(HttpMethod)	| Applies the policy to a specific HTTP method.|
|ForPath(path)	| Applies the policy to a specific route.|
|WithGlobalPolicy(configurePolicy)| Applies a global policy to all endpoints, with the specified configuration.|

## 📝 Examples

The API project provides a live example of the rate limiter in use, but some common use cases are covered below.

### 💡 IP and API Key rate limit applied to a specific endpoint
The rate limiter will apply the following rules to `GET` requests made to the `/WeatherForecast` endpoint: 
- 2 requests per 10 seconds based on the `X-Api-Key` header.
- 10 requests per hour based on the IP address.

If one of the limits is reached, then a `429 - TooManyRequests` response is returned. 

```csharp
app.UseInMemoryRateLimiter(options =>
        {
            options.AddPolicy("ApiKey",
                policy =>
                {
                    policy
                        .FixedWindow(2, TimeSpan.FromSeconds(10))
                        .WithClientIdLimit("X-Api-Key");
                });
            
            options.AddPolicy("IpAddress", policy =>
            {
                policy
                    .FixedWindow(10, TimeSpan.FromHours(1))
                    .WithIpLimit();
            });
            
            options.ConfigureEndpoint(endpoint =>
            {
                endpoint
                    .ForMethod(HttpMethod.Get)
                    .ForPath("/WeatherForecast")
                    .WithPolicy("ApiKey")
                    .WithPolicy("IpAddress");
            });
        });
```

### 💡 Apply a global policy to all endpoints
The following global policy will be applied to all request Patsh and HTTP Methods:
- 50 requests per day based on the IP address.

This can be used as a fallback, or in simple use cases. The Global Policy will work alongside any specified endpoint policies.

If the limit is reached, then a `429 - TooManyRequests` response is returned. 

```csharp
app.UseInMemoryRateLimiter(options =>
        {            
            options.WithGlobalPolicy(policy =>
            {
                policy
                    .FixedWindow(50, TimeSpan.FromDays(1))
                    .WithIpLimit();
            });
        });
```

### 💡 Override the default response when a rate limit is exceeded
The default response of the rate limiter when a limit is exceeded can be overwritten. This allows you to edit the HttpContext to allow for different requirements, such as an alternative HTTP Response Code.

```csharp
options.OnLimitExceeded(context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            });
```
