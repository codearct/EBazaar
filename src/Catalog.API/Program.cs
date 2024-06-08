var builder = WebApplication.CreateBuilder(args);

#region Services

var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(assembly);

//TODO:If Carter would has registeration way from assembly like MediatR
//then use that way and move to BuildingBlocks
builder.Services.AddCarter();

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
