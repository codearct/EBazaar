var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

#endregion

var app = builder.Build();

#region Pipeline

app.MapReverseProxy();

#endregion

app.Run();
