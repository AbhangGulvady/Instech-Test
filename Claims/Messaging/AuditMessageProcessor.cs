using Claims.Auditing;
using Microsoft.Extensions.Hosting;

namespace Claims.Messaging
{
    // This is related to background processor only
    public sealed class AuditMessageProcessor : BackgroundService
    {
        private readonly IAuditMessageBus _bus;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuditMessageProcessor> _logger;

        public AuditMessageProcessor(
            IAuditMessageBus bus,
            IServiceScopeFactory scopeFactory,
            ILogger<AuditMessageProcessor> logger)
        {
            _bus = bus;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var message in _bus.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await PersistAsync(message, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to persist audit message for {EntityType} {EntityId} ({HttpRequestType}).",
                        message.EntityType, message.EntityId, message.HttpRequestType);
                }
            }
        }

        private async Task PersistAsync(AuditMessage message, CancellationToken cancellationToken)
        {
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

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
