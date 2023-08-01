using GoratLoans.Infrastructure;

namespace GoratLoans.Customers.HostedServices;

internal class CustomersHostedService : IHostedService
{
    private const string CustomerAppliedQueueName = "gorat-loans-customers-customer-applied";
    private const string PublicEventBus = "gorat-loans-public-events";

    private readonly ILogger<CustomersHostedService> _logger;
    private readonly IMessageBroker _messageBroker;


    public CustomersHostedService(ILogger<CustomersHostedService> logger, IMessageBroker messageBroker)
    {
        _logger = logger;
        _messageBroker = messageBroker;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting service....");
            
        //_messageBroker.SubscribeTo(CustomerAppliedQueueName, PublicEventBus, onMessage);
            
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping service....");
            
        _messageBroker.Dispose();
            
        return Task.CompletedTask;
    }
}