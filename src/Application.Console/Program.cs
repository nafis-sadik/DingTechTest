using DingTechTest.Configurations;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Add services to the container.
builder.Services.RosolveDependencies(builder.Configuration);

using IHost host = builder.Build();

await host.InitApplicationDatabase();

Console.WriteLine("Application Started!");

// Keep running or do work here
await host.RunAsync();
