var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var api = builder.AddProject<Projects.GoodHamburger_Api>("api")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.GoodHamburger_Web>("web")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();