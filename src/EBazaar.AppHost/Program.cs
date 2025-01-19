var builder = DistributedApplication.CreateBuilder(args);

var catalogDb = builder.AddPostgres("catalog-db");

var basketDb = builder.AddPostgres("basket-db");

var distributedCache = builder.AddRedis("distributed-cache");

var orderDb = builder.AddSqlServer("order-db");

var password = builder.AddParameter("password", "guest");
var messageBroker = builder.AddRabbitMQ("message-broker", password: password)
    .WithHttpEndpoint(5672, 5672)
    .WithEnvironment("MessageBroker__Host", "amqp://localhost:5672")
    .WithEnvironment("MessageBroker__UserName", "guest")
    .WithEnvironment("MessageBroker__Password", "guest")
    .WithManagementPlugin();

var catalogApi = builder.AddProject<Projects.Catalog_API>("catalog-api")
    .WithReference(catalogDb, "Database")
    .WaitFor(catalogDb);

var discountGrpc = builder.AddProject<Projects.Discount_Grpc>("discount-grpc");

var basketApi = builder.AddProject<Projects.Basket_API>("basket-api")
    .WithReference(basketDb, "Database")
    .WithReference(distributedCache, "Redis")
    .WithReference(messageBroker)
    .WithReference(discountGrpc)
    .WaitFor(basketDb)
    .WaitFor(messageBroker)
    .WaitFor(discountGrpc);

var orderingApi = builder.AddProject<Projects.Ordering_API>("ordering-api")
    .WithReference(orderDb, "Database")
    .WithReference(messageBroker)
    .WaitFor(orderDb)
    .WaitFor(messageBroker);

var yarpApiGateway = builder.AddProject<Projects.YarpApiGateway>("yarpapigateway")
    .WithReference(catalogApi)
    .WithReference(basketApi)
    .WithReference(orderingApi);

builder.AddProject<Projects.Ebazaar_Web>("ebazaar-web")
    .WithReference(yarpApiGateway);

builder.Build().Run();
