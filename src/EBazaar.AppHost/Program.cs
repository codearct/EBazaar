var builder = DistributedApplication.CreateBuilder(args);

var catalogDb = builder.AddPostgres("catalog-db")
    .WithEnvironment("POSTGRES_DB", "CatalogDb")
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_PASSWORD", "postgres")
    .WithHttpEndpoint(5432)
    .WithDataVolume("postgres_catalog")
    .WithPgAdmin();

var basketDb = builder.AddPostgres("basket-db")
    .WithEnvironment("POSTGRES_DB", "BasketDb")
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_PASSWORD", "postgres")
    .WithHttpEndpoint(5433)
    .WithDataVolume("postgres_basket")
    .WithPgAdmin();

var distributedCache = builder.AddRedis("distributed-cache")
    .WithHttpEndpoint(6379);

var orderDb = builder.AddSqlServer("order-db")
    .WithEnvironment("SA_PASSWORD", "SwN12345678")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithHttpEndpoint(1433);

var messageBroker = builder.AddRabbitMQ("message-broker")
    .WithEnvironment("RABBITMQ_DEFAULT_USER", "guest")
    .WithEnvironment("RABBITMQ_DEFAULT_PASS", "guest")
    .WithEnvironment("HOSTNAME", "ebazaar-mq")
    .WithHttpEndpoint(5672)
    .WithHttpsEndpoint(15672);

var catalogApi = builder.AddProject<Projects.Catalog_API>("catalog-api")
    .WithReference(catalogDb)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_HTTP_PORTS", "8080")
    .WithEnvironment("ASPNETCORE_HTTPS_PORTS", "8081")
    .WithEnvironment("ConnectionStrings__Database", "Server=catalog-db;Port=5432;Database=CatalogDb;User Id=postgres;Password=postgres;Include Error Detail=true")
    .WithHttpEndpoint(6000)
    .WithHttpsEndpoint(6060)
    .WaitFor(catalogDb);

var discountGrpc = builder.AddProject<Projects.Discount_Grpc>("discount-grpc")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_HTTP_PORTS", "8080")
    .WithEnvironment("ASPNETCORE_HTTPS_PORTS", "8081")
    .WithEnvironment("ConnectionStrings__Database", "Data Source=discountdb")
    .WithHttpEndpoint(6002)
    .WithHttpsEndpoint(6062);

var basketApi = builder.AddProject<Projects.Basket_API>("basket-api")
    .WithReference(basketDb)
    .WithReference(distributedCache)
    .WithReference(messageBroker)
    .WithReference(discountGrpc)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_HTTP_PORTS", "8080")
    .WithEnvironment("ASPNETCORE_HTTPS_PORTS", "8081")
    .WithEnvironment("ConnectionStrings__Database", "Server=basket-db;Port=5432;Database=BasketDb;User Id=postgres;Password=postgres;Include Error Detail=true")
    .WithEnvironment("ConnectionStrings__Redis", "distributed-cache:6379")
    .WithEnvironment("GrpcSettings__DiscountUrl", "http://discount-grpc:8081")
    .WithEnvironment("MessageBroker__Host", "amqp://ebazaar-mq:567")
    .WithEnvironment("MessageBroker__UserNam", "guest")
    .WithEnvironment("MessageBroker__Password", "guest")
    .WithHttpEndpoint(6001)
    .WithHttpsEndpoint(6061)
    .WaitFor(basketDb)
    .WaitFor(distributedCache)
    .WaitFor(discountGrpc)
    .WaitFor(messageBroker);

var orderingApi = builder.AddProject<Projects.Ordering_API>("ordering-api")
    .WithReference(orderDb)
    .WithReference(messageBroker)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_HTTP_PORTS", "8080")
    .WithEnvironment("ASPNETCORE_HTTPS_PORTS", "8081")
    .WithEnvironment("ConnectionStrings__Database", "Server=order-db;Database=OrderDb;User ID=sa;Password=SwN12345678;Encrypt=False;TrustServerCertificate=True")
    .WithEnvironment("MessageBroker__Host", "amqp://ebazaar-mq:567")
    .WithEnvironment("MessageBroker__UserNam", "guest")
    .WithEnvironment("MessageBroker__Password", "guest")
    .WithHttpEndpoint(6003)
    .WithHttpsEndpoint(6063)
    .WaitFor(orderDb)
    .WaitFor(messageBroker);

var yarpApiGateway = builder.AddProject<Projects.YarpApiGateway>("yarpapigateway")
    .WithReference(catalogApi)
    .WithReference(basketApi)
    .WithReference(orderingApi)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_HTTP_PORTS", "8080")
    .WithEnvironment("ASPNETCORE_HTTPS_PORTS", "8081")
    .WithHttpEndpoint(6004)
    .WithHttpsEndpoint(6064)
    .WaitFor(catalogApi)
    .WaitFor(basketApi)
    .WaitFor(orderingApi);

builder.AddProject<Projects.Ebazaar_Web>("ebazaar-web")
    .WithReference(yarpApiGateway)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_HTTP_PORTS", "8080")
    .WithEnvironment("ASPNETCORE_HTTPS_PORTS", "8081")
    .WithEnvironment("ApiSettings__GatewayAddress", "http://yarpapigateway:8080")
    .WithHttpEndpoint(6005)
    .WithHttpsEndpoint(6065)
    .WaitFor(yarpApiGateway);

builder.Build().Run();
