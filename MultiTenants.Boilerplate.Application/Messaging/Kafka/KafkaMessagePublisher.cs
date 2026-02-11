using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MultiTenants.Boilerplate.Application.Messaging.Kafka;

/// <summary>
/// Kafka implementation of IMessagePublisher
/// </summary>
public class KafkaMessagePublisher : IMessagePublisher, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaMessagePublisher> _logger;

    public KafkaMessagePublisher(IProducer<string, string> producer, ILogger<KafkaMessagePublisher> logger)
    {
        _producer = producer;
        _logger = logger;
    }

    public Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default) where T : class
    {
        return PublishAsync(topic, string.Empty, message, cancellationToken);
    }

    public async Task PublishAsync<T>(string topic, string routingKey, T message, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var kafkaMessage = new Message<string, string>
            {
                Key = routingKey,
                Value = json
            };

            var result = await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);
            _logger.LogDebug("Published message to topic: {Topic}, partition: {Partition}, offset: {Offset}",
                topic, result.Partition, result.Offset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message to topic: {Topic}", topic);
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
    }
}
