namespace Claims.Services
{
    /// <summary>
    /// Records audit entries for changes made to claims and covers.
    /// </summary>
    /// <remarks>
    /// Implementations are expected to be non-blocking with respect to the
    /// audit data store. The default implementation publishes an audit message
    /// to an in-memory bus that is drained asynchronously by a background worker.
    /// </remarks>
    public interface IAuditService
    {
        /// <summary>
        /// Audits an HTTP operation performed against a claim.
        /// </summary>
        /// <param name="id">The identifier of the affected claim.</param>
        /// <param name="httpRequestType">The HTTP request method, e.g. <c>POST</c> or <c>DELETE</c>.</param>
        /// <param name="cancellationToken">A token to cancel the publish.</param>
        ValueTask AuditClaimAsync(string id, string httpRequestType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Audits an HTTP operation performed against a cover.
        /// </summary>
        /// <param name="id">The identifier of the affected cover.</param>
        /// <param name="httpRequestType">The HTTP request method, e.g. <c>POST</c> or <c>DELETE</c>.</param>
        /// <param name="cancellationToken">A token to cancel the publish.</param>
        ValueTask AuditCoverAsync(string id, string httpRequestType, CancellationToken cancellationToken = default);
    }
}
