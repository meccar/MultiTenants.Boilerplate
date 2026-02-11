namespace MultiTenants.Boilerplate.Application.Messaging;

/// <summary>
/// Interface for consuming messages from a message broker
/// </summary>
public interface IMessageConsumer
{
    /// <summary>
    /// Subscribes to a topic/queue and processes messages
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    /// <param name="topic">The topic/queue name</param>
    /// <param name="handler">The message handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SubscribeAsync<T>(string topic, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Subscribes to a topic/queue with a consumer group
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    /// <param name="topic">The topic/queue name</param>
    /// <param name="consumerGroup">The consumer group name</param>
    /// <param name="handler">The message handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SubscribeAsync<T>(string topic, string consumerGroup, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where T : class;
}
