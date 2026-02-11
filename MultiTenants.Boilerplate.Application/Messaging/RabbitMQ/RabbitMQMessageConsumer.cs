using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MultiTenants.Boilerplate.Application.Messaging.RabbitMQ;

/// <summary>
/// RabbitMQ implementation of IMessageConsumer
/// </summary>
public class RabbitMQMessageConsumer : IMessageConsumer, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQMessageConsumer> _logger;
    private readonly List<string> _consumerTags = new();

    public RabbitMQMessageConsumer(IConnection connection, ILogger<RabbitMQMessageConsumer> logger)
    {
        _connection = connection;
        _channel = _connection.CreateModel();
        _logger = logger;
    }

    public Task SubscribeAsync<T>(string topic, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where T : class
    {
        return SubscribeAsync(topic, string.Empty, handler, cancellationToken);
    }

    public Task SubscribeAsync<T>(string topic, string consumerGroup, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            // Declare exchange
            _channel.ExchangeDeclare(topic, ExchangeType.Topic, durable: true, autoDelete: false);

            // Declare queue
            var queueName = string.IsNullOrEmpty(consumerGroup) ? $"{topic}_queue" : $"{topic}_{consumerGroup}_queue";
            _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);

            // Bind queue to exchange
            _channel.QueueBind(queueName, topic, string.Empty);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<T>(json);

                    if (message != null)
                    {
                        await handler(message, cancellationToken);
                        _channel.BasicAck(ea.DeliveryTag, false);
                        _logger.LogDebug("Processed message from queue: {QueueName}", queueName);
                    }
                    else
                    {
                        _logger.LogWarning("Received null message from queue: {QueueName}", queueName);
                        _channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from queue: {QueueName}", queueName);
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            var consumerTag = _channel.BasicConsume(queueName, autoAck: false, consumer);
            _consumerTags.Add(consumerTag);

            _logger.LogInformation("Subscribed to queue: {QueueName}", queueName);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to topic: {Topic}", topic);
            throw;
        }
    }

    public void Dispose()
    {
        foreach (var consumerTag in _consumerTags)
        {
            _channel?.BasicCancel(consumerTag);
        }
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
