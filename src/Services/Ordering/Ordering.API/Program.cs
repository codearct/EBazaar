using Ordering.API;
using Ordering.Application;
using Ordering.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices();

#endregion


var app = builder.Build();


#region Pipeline

app.UseApiServices();

#endregion

app.Run();
