namespace MultiTenants.Boilerplate.Application.Messaging;

/// <summary>
/// Interface for publishing messages to a message broker
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Publishes a message to the specified topic/exchange
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    /// <param name="topic">The topic/exchange name</param>
    /// <param name="message">The message to publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Publishes a message with routing key
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    /// <param name="topic">The topic/exchange name</param>
    /// <param name="routingKey">The routing key</param>
    /// <param name="message">The message to publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task PublishAsync<T>(string topic, string routingKey, T message, CancellationToken cancellationToken = default) where T : class;
}
