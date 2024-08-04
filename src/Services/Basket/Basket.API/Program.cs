var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services.AddCarter();

var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

#endregion

var app = builder.Build();

#region Pipelines

app.MapCarter();

#endregion

app.Run();
