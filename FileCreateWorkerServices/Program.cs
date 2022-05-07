using FileCreateWorkerServices;
using FileCreateWorkerServices.Models;
using FileCreateWorkerServices.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) => 
    {
        services.AddHostedService<Worker>();

        services.AddDbContext<AdventureWorks2019Context>(
        options => options.UseSqlServer(hostContext.Configuration.GetConnectionString("SqlServer")));

        services.AddSingleton<RabbitMQClientService>();

        services.AddSingleton(sp => new ConnectionFactory()
        {
            Uri = new Uri(hostContext.Configuration.GetConnectionString("RabbitMQ")),
            DispatchConsumersAsync = true
        });
    })
    .Build();

await host.RunAsync();
