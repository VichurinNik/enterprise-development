using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var mysql = builder.AddMySql("mysql").WithImageTag("8.4");

if (!builder.Configuration.GetValue<bool>("Testing:DisableDataVolume"))
    mysql.WithDataVolume();

var museum = mysql.AddDatabase("museum");

builder.AddProject<Projects.Museum_Api>("api", launchProfileName: "http")
    .WithReference(museum)
    .WaitFor(museum)
    .WithHttpHealthCheck("/health")
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Scalar";
        url.Url = "/scalar/v1";
    });

builder.Build().Run();
