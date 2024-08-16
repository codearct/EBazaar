using BuildingBlocks.Exceptions.Handler;

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

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
    opts.Schema.For<ShoppingCart>().Identity(x => x.UserName);
}).UseLightweightSessions();

builder.Services.AddScoped<IBasketRepository,BasketRepository>();
builder.Services.Decorate<IBasketRepository,CachedBasketRepository>();//Why didn't we use AddKeyedScope?

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    //options.InstanceName = "Basket";
});

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

#endregion

var app = builder.Build();

#region Pipelines

app.MapCarter();

app.UseExceptionHandler(options => { });

#endregion

app.Run();
