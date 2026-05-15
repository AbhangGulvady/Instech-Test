using Claims.Messaging;

namespace Claims.Services
{
    
    public class AuditService : IAuditService
    {
        private readonly IAuditMessageBus _bus;

        public AuditService(IAuditMessageBus bus)
        {
            _bus = bus;
        }

        public ValueTask AuditClaimAsync(string id, string httpRequestType, CancellationToken cancellationToken = default)
        {
            var valueTask = _bus.PublishAsync(
                new AuditMessage(AuditEntityType.Claim, id, httpRequestType, DateTime.UtcNow),
                cancellationToken);

            return valueTask;
        }


        public ValueTask AuditCoverAsync(string id, string httpRequestType, CancellationToken cancellationToken = default)
        {
            var valueTask = _bus.PublishAsync(
                    new AuditMessage(AuditEntityType.Cover, id, httpRequestType, DateTime.UtcNow),
                    cancellationToken);

            return valueTask;

        } 
    }
}
