using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MultiTenants.Boilerplate.Application.Messaging.RabbitMQ;

/// <summary>
/// RabbitMQ implementation of IMessagePublisher
/// </summary>
public class RabbitMQMessagePublisher : IMessagePublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQMessagePublisher> _logger;

    public RabbitMQMessagePublisher(IConnection connection, ILogger<RabbitMQMessagePublisher> logger)
    {
        _connection = connection;
        _channel = _connection.CreateModel();
        _logger = logger;
    }

    public Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default) where T : class
    {
        return PublishAsync(topic, string.Empty, message, cancellationToken);
    }

    public Task PublishAsync<T>(string topic, string routingKey, T message, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            // Declare exchange if it doesn't exist
            _channel.ExchangeDeclare(topic, ExchangeType.Topic, durable: true, autoDelete: false);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";

            _channel.BasicPublish(
                exchange: topic,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            _logger.LogDebug("Published message to exchange: {Exchange}, routing key: {RoutingKey}", topic, routingKey);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message to exchange: {Exchange}", topic);
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
