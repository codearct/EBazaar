var builder = WebApplication.CreateBuilder(args);

#region Services

//TODO:If Carter would has registeration way from assembly like MediatR
//then use that way and move to BuildingBlocks
builder.Services.AddCarter();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddMarten(op =>
{
    op.Connection(builder.Configuration.GetConnectionString("Database")!);
}).UseLightweightSessions();
#endregion

var app = builder.Build();

#region Pipelines

app.MapCarter();

#endregion

app.Run();
