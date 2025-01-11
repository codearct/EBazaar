using Discount.Grpc.Data;
using Discount.Grpc.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
var dbConnection = builder.Configuration.GetConnectionString("Database");

#region Services
builder.Services.AddGrpc();

builder.Services.AddDbContext<DiscountContext>(options =>
{
    options.UseSqlite(dbConnection);
});
#endregion

var app = builder.Build();

app.MapDefaultEndpoints();

#region Pipeline
app.UseMigration();

app.MapGrpcService<DiscountService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
#endregion

app.Run();
