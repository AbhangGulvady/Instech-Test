namespace Claims.Messaging
{
    public interface IAuditMessageBus
    {
        ValueTask PublishAsync(AuditMessage message, CancellationToken cancellationToken = default);
        IAsyncEnumerable<AuditMessage> ReadAllAsync(CancellationToken cancellationToken);
    }
}
