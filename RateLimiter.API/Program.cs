using System.Net;
using RateLimiter.Extensions;

namespace RateLimiter.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Add services to the container.
        
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        var app = builder.Build();
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        // Testing configuration of rate limiter policies. 
        app.UseRateLimiter(options =>
        {
            options.AddPolicy("ClientId",
                policy =>
                {
                    policy
                        .FixedWindow(20, TimeSpan.FromHours(1))
                        .WithClientIdLimit("X-Client-Id");
                    
                });
            
            options.AddPolicy("ApiKey",
                policy =>
                {
                    policy
                        .FixedWindow(100, TimeSpan.FromHours(1))
                        .WithClientIdLimit("X-Api-Key");
                });
            
            options.WithGlobalPolicy(policy =>
            {
                policy
                    .FixedWindow(100, TimeSpan.FromHours(1))
                    .WithIpLimit();
            });
                
            options.OnLimitExceeded(((context) =>
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }));
            
            options.ConfigureEndpoint(endpoint =>
            {
                endpoint.ForMethod(HttpMethod.Get)
                    .ForPath("/api/weather")
                    .WithPolicy("ClientId")
                    .WithPolicy("ApiKey");
            });
            
        });
        
        app.UseHttpsRedirection();
        
        app.UseAuthorization();
        
        app.MapControllers();
        
        app.Run();
    }
}