namespace Claims.Messaging
{
    public enum AuditEntityType
    {
        Claim,
        Cover
    }

    public record AuditMessage(
        AuditEntityType EntityType,
        string EntityId,
        string HttpRequestType,
        DateTime OccurredAtUtc);
}
