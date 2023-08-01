using GoratLoans.Infrastructure;
using GoratLoans.Infrastructure.Extensions;
using GoratLoans.Customers.HostedServices;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((context, builder) =>
    {
        builder.AddConsole();
    })
    .ConfigureAppConfiguration(((context, builder) =>
    {
        builder.AddEnvironmentVariables();
        var env = context.HostingEnvironment;

        builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                optional: true, reloadOnChange: true);
    }))
    .ConfigureServices(services =>
    {
        services.AddRabbitMq(new MessageBrokerConfiguration{ HostName = "Oxygen"});
        services.AddHostedService<CustomersHostedService>();
    })
    .Build();
    
await host.StartAsync();