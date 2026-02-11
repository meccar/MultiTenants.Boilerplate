using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MultiTenants.Boilerplate.Application.Messaging.Kafka;

/// <summary>
/// Kafka implementation of IMessageConsumer
/// </summary>
public class KafkaMessageConsumer : IMessageConsumer, IDisposable
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaMessageConsumer> _logger;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public KafkaMessageConsumer(IConsumer<string, string> consumer, ILogger<KafkaMessageConsumer> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    public Task SubscribeAsync<T>(string topic, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where T : class
    {
        return SubscribeAsync(topic, "default-group", handler, cancellationToken);
    }

    public async Task SubscribeAsync<T>(string topic, string consumerGroup, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            _consumer.Subscribe(topic);

            _logger.LogInformation("Subscribed to topic: {Topic}, consumer group: {ConsumerGroup}", topic, consumerGroup);

            // Start consuming in background
            _ = Task.Run(async () =>
            {
                try
                {
                    while (!cancellationToken.IsCancellationRequested && !_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        try
                        {
                            var result = _consumer.Consume(TimeSpan.FromSeconds(1));
                            if (result != null)
                            {
                                try
                                {
                                    var message = JsonSerializer.Deserialize<T>(result.Message.Value);
                                    if (message != null)
                                    {
                                        await handler(message, cancellationToken);
                                        _consumer.Commit(result);
                                        _logger.LogDebug("Processed message from topic: {Topic}, partition: {Partition}, offset: {Offset}",
                                            topic, result.Partition, result.Offset);
                                    }
                                    else
                                    {
                                        _logger.LogWarning("Received null message from topic: {Topic}", topic);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error processing message from topic: {Topic}", topic);
                                }
                            }
                        }
                        catch (ConsumeException ex)
                        {
                            _logger.LogError(ex, "Error consuming from topic: {Topic}", topic);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in consumer loop for topic: {Topic}", topic);
                }
            }, cancellationToken);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to topic: {Topic}", topic);
            throw;
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _consumer?.Close();
        _consumer?.Dispose();
        _cancellationTokenSource?.Dispose();
    }
}
