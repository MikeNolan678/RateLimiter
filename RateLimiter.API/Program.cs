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
        
        builder.Services.AddInMemoryRateLimiter();
        
        var app = builder.Build();
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        // Testing configuration of rate limiter policies. 
        app.UseInMemoryRateLimiter(options =>
        {
            options.AddPolicy("ApiKey",
                policy =>
                {
                    policy
                        .FixedWindow(2, TimeSpan.FromSeconds(10))
                        .WithClientIdLimit("X-Api-Key");
                });
            
            //TODO: Ensure that the policies are applied in the correct order, currently the order is reversed.
            
            options.ConfigureEndpoint(endpoint =>
            {
                endpoint.ForMethod(HttpMethod.Get)
                    .ForPath("/WeatherForecast")
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