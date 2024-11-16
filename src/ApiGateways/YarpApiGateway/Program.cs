using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(10);
        options.PermitLimit = 5;
    });
});
#endregion

var app = builder.Build();

#region Pipeline

app.UseRateLimiter();

app.MapReverseProxy();

#endregion

app.Run();
