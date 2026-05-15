using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Claims.Auditing;
using Claims.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Claims.Messaging;

// Azure Service Bus Background service.
public sealed class AzureServiceBusAuditConsumer : BackgroundService
{
    private readonly ServiceBusClient _client;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AzureServiceBusAuditConsumer> _logger;
    private readonly string _queueName;
    private ServiceBusProcessor? _processor;

    public AzureServiceBusAuditConsumer(
        ServiceBusClient client,
        IServiceScopeFactory scopeFactory,
        IOptions<AzureServiceBusOptions> options,
        ILogger<AzureServiceBusAuditConsumer> logger)
    {
        _client = client;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _queueName = options.Value.QueueName;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor = _client.CreateProcessor(_queueName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false,
            MaxConcurrentCalls = 4
        });

        _processor.ProcessMessageAsync += HandleMessageAsync;
        _processor.ProcessErrorAsync  += args =>
        {
            _logger.LogError(args.Exception, "Service Bus error on {Entity}", args.EntityPath);
            return Task.CompletedTask;
        };

        await _processor.StartProcessingAsync(stoppingToken);

        // Keep the service alive until cancellation.
        try { await Task.Delay(Timeout.Infinite, stoppingToken); }
        catch (TaskCanceledException) { /* shutdown */ }

        await _processor.StopProcessingAsync(CancellationToken.None);
    }

    private async Task HandleMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var message = JsonSerializer.Deserialize<AuditMessage>(args.Message.Body.ToArray())
                          ?? throw new InvalidOperationException("Empty audit message body.");

            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AuditContext>();

            switch (message.EntityType)
            {
                case AuditEntityType.Claim:
                    context.Add(new ClaimAudit
                    {
                        ClaimId = message.EntityId,
                        Created = message.OccurredAtUtc,
                        HttpRequestType = message.HttpRequestType
                    });
                    break;
                case AuditEntityType.Cover:
                    context.Add(new CoverAudit
                    {
                        CoverId = message.EntityId,
                        Created = message.OccurredAtUtc,
                        HttpRequestType = message.HttpRequestType
                    });
                    break;
            }

            await context.SaveChangesAsync(args.CancellationToken);
            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to persist audit message {MessageId}", args.Message.MessageId);
            await args.AbandonMessageAsync(args.Message, cancellationToken: args.CancellationToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_processor is not null)
            await _processor.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}