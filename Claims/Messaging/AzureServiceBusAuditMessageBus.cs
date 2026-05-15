using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Claims.Configuration;
using Microsoft.Extensions.Options;

namespace Claims.Messaging;

// In future, we consider abstracting the message bus interface further
// to support multiple providers (e.g., RabbitMQ, Kafka) without changing the service layer.
// For now, we implement it directly for Azure Service Bus as it's our current choice for messaging infrastructure.
public sealed class AzureServiceBusAuditMessageBus : IAuditMessageBus, IAsyncDisposable
{
    private readonly ServiceBusSender _sender;
    private readonly ServiceBusClient _client;

    public AzureServiceBusAuditMessageBus(ServiceBusClient client, IOptions<AzureServiceBusOptions> options)
    {
        _client = client;
        _sender = client.CreateSender(options.Value.QueueName);
    }

    public async ValueTask PublishAsync(AuditMessage message, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.SerializeToUtf8Bytes(message);
        var sbMessage = new ServiceBusMessage(payload)
        {
            ContentType = "application/json",
            Subject = message.EntityType.ToString()
        };

        await _sender.SendMessageAsync(sbMessage, cancellationToken);
    }

    // For now, we do not support reading messages through this bus.
    // The AzureServiceBusAuditConsumer will handle message consumption directly from the queue.
    // this is just to implment azureservice bus.
    public IAsyncEnumerable<AuditMessage> ReadAllAsync(CancellationToken cancellationToken)
        => throw new NotSupportedException(
            "ReadAllAsync is not supported when using Azure Service Bus. " +
            "AzureServiceBusAuditConsumer drains the queue directly.");

    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _client.DisposeAsync();
    }
}