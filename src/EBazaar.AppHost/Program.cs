var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.YarpApiGateway>("yarpapigateway");

builder.AddProject<Projects.Basket_API>("basket-api");

builder.AddProject<Projects.Catalog_API>("catalog-api");

builder.AddProject<Projects.Discount_Grpc>("discount-grpc");

builder.AddProject<Projects.Ordering_API>("ordering-api");

builder.AddProject<Projects.Ebazaar_Web>("ebazaar-web");

builder.Build().Run();
