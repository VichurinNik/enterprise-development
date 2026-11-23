using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var mysql = builder.AddMySql("mysql").WithImageTag("8.4");

var isTestRun = builder.Configuration.GetValue<bool>("Testing:DisableDataVolume");
if (!isTestRun)
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

if (isTestRun)
{
    builder.AddProject<Projects.Museum_Api>("api-second", launchProfileName: "http")
        .WithReference(museum)
        .WaitFor(museum)
        .WithHttpHealthCheck("/health")
        .WithExplicitStart();
}

builder.Build().Run();
