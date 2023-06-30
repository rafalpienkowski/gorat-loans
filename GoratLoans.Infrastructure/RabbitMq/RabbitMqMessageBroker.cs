using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GoratLoans.Infrastructure.RabbitMq;

internal class RabbitMqMessageBroker : IMessageBroker
{
    private readonly MessageBrokerConfiguration _configuration;
    private readonly ILogger<RabbitMqMessageBroker> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqMessageBroker(ILogger<RabbitMqMessageBroker> logger, MessageBrokerConfiguration configuration)
    {
        _configuration = configuration;
        _logger = logger;

        IConnectionFactory connectionFactory = new ConnectionFactory
        {
            HostName = _configuration.HostName
        };
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void SubscribeTo<TIn, TOut>(string inQueueName, string outQueueName, Func<TIn, TOut> onMessage)
        where TIn : class, new() where TOut : class, new()
    {
        if (onMessage == null)
        {
            throw new ArgumentNullException(nameof(onMessage));
        }
        
        var consumer = OnMessage(ea => outQueueName, ea =>
        {
            var props = _channel.CreateBasicProperties();
            props.CorrelationId = ea.BasicProperties.CorrelationId;

            return props;
        }, onMessage);

        _channel.BasicConsume(queue: inQueueName, autoAck: false, consumer);

        _logger.LogInformation("Subscribed for messages. Waiting for request on {QueueName}", inQueueName);
    }

    private EventingBasicConsumer OnMessage<TIn, TOut>(Func<BasicDeliverEventArgs, string> outQueueName,
        Func<BasicDeliverEventArgs, IBasicProperties> replyProperties, Func<TIn, TOut> handleMessage)
        where TIn : class, new() where TOut : class, new()
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (model, ea) =>
        {
            var response = default(TOut);
            var props = ea.BasicProperties;

            _logger.LogTrace("Message received. Correlation Id: {CorrelationId}", props.CorrelationId);

            var failed = false;

            try
            {
                var request = DeserializeMessage<TIn>(ea.Body.ToArray());

                response = handleMessage(request);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong");
                failed = true;
            }
            finally
            {
                if (failed)
                {
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                    _logger.LogError($"Message NACK....");
                }
                else
                {
                    var replyProps = replyProperties(ea);

                    var messageBytes = SerializeMessage(response);

                    _channel.BasicPublish(exchange: "", routingKey: outQueueName(ea), basicProperties: replyProps,
                        body: messageBytes);
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                    _logger.LogTrace("Response send. Queue: {OutQueueName}, Correlation Id: {CorrelationId}", 
                        outQueueName(ea), replyProps.CorrelationId);
                }
            }
        };

        return consumer;
    }

    private static TIn DeserializeMessage<TIn>(byte[] body) where TIn : class, new()
    {
        var message = Encoding.UTF8.GetString(body);
        var request = JsonConvert.DeserializeObject<TIn>(message);
        if (request == null)
        {
            throw new ArgumentNullException(nameof(body), "Unable to serialize request");
        }

        return request;
    }

    private static byte[] SerializeMessage(object? message)
    {
        if (message == null)
        {
            return Array.Empty<byte>();
        }
        
        var messageString = JsonConvert.SerializeObject(message);
        var messageBytes = Encoding.UTF8.GetBytes(messageString);

        return messageBytes;
    }

    public void Dispose()
    {
        _logger.LogInformation("Stop listening to {HostName}....", _configuration.HostName);
        _connection?.Dispose();
        _channel?.Dispose();
    }
}