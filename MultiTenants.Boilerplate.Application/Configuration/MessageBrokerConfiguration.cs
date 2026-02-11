using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MultiTenants.Boilerplate.Application.Messaging;
using MultiTenants.Boilerplate.Application.Messaging.Kafka;
using MultiTenants.Boilerplate.Application.Messaging.RabbitMQ;
using RabbitMQ.Client;
using Confluent.Kafka;

namespace MultiTenants.Boilerplate.Application.Configuration;

/// <summary>
/// Configuration extensions for message broker services
/// </summary>
public static class MessageBrokerConfiguration
{
    /// <summary>
    /// Adds message broker services (RabbitMQ or Kafka) based on configuration
    /// </summary>
    public static IServiceCollection AddMessageBroker(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var brokerType = configuration["MessageBroker:Type"]?.ToLowerInvariant();

        switch (brokerType)
        {
            case "rabbitmq":
                services.AddRabbitMQ(configuration);
                break;
            case "kafka":
                services.AddKafka(configuration);
                break;
            default:
                // No message broker configured
                break;
        }

        return services;
    }

    private static IServiceCollection AddRabbitMQ(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RabbitMQ")
            ?? configuration["MessageBroker:RabbitMQ:ConnectionString"]
            ?? "amqp://guest:guest@localhost:5672/";

        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString),
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        var connection = factory.CreateConnection();
        services.AddSingleton(connection);
        services.AddSingleton<IMessagePublisher, RabbitMQMessagePublisher>();
        services.AddSingleton<IMessageConsumer, RabbitMQMessageConsumer>();

        return services;
    }

    private static IServiceCollection AddKafka(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var bootstrapServers = configuration["MessageBroker:Kafka:BootstrapServers"]
            ?? configuration.GetConnectionString("Kafka")
            ?? "localhost:9092";

        // Producer configuration
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 100
        };

        services.AddSingleton<IProducer<string, string>>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<KafkaMessagePublisher>>();
            return new ProducerBuilder<string, string>(producerConfig)
                .SetErrorHandler((_, e) => logger.LogError("Kafka producer error: {Error}", e.Reason))
                .Build();
        });

        // Consumer configuration
        var consumerGroup = configuration["MessageBroker:Kafka:ConsumerGroup"] ?? "default-group";
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = consumerGroup,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        services.AddSingleton<IConsumer<string, string>>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<KafkaMessageConsumer>>();
            return new ConsumerBuilder<string, string>(consumerConfig)
                .SetErrorHandler((_, e) => logger.LogError("Kafka consumer error: {Error}", e.Reason))
                .Build();
        });

        services.AddSingleton<IMessagePublisher, KafkaMessagePublisher>();
        services.AddSingleton<IMessageConsumer, KafkaMessageConsumer>();

        return services;
    }
}
