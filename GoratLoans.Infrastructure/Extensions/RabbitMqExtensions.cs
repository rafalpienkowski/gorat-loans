using GoratLoans.Infrastructure.RabbitMq;
using Microsoft.Extensions.DependencyInjection;

namespace GoratLoans.Infrastructure.Extensions;

public static class RabbitMqExtensions
{
    public static void AddRabbitMq(this IServiceCollection services, MessageBrokerConfiguration configuration)
    {
        services.AddSingleton(configuration);
        services.AddSingleton<IMessageBroker, RabbitMqMessageBroker>();
    }
}