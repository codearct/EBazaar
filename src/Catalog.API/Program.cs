var builder = WebApplication.CreateBuilder(args);

#region Services

//TODO:If Carter would has registeration way from assembly like MediatR then use that way.
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

#endregion

var app = builder.Build();

#region Pipelines

app.MapCarter();

#endregion

app.Run();
