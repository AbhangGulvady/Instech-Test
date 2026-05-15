namespace Claims.Messaging
{
    /// <summary>
    /// The kind of entity that an <see cref="AuditMessage"/> refers to.
    /// </summary>
    public enum AuditEntityType
    {
        /// <summary>The audit entry refers to a <see cref="Claim"/>.</summary>
        Claim,

        /// <summary>The audit entry refers to a <see cref="Cover"/>.</summary>
        Cover
    }

    public record AuditMessage(
        AuditEntityType EntityType,
        string EntityId,
        string HttpRequestType,
        DateTime OccurredAtUtc);
}
