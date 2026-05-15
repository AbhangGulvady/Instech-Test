namespace Claims.Repositories
{
    /// <summary>
    /// Provides persistence operations for <see cref="Claim"/> entities.
    /// </summary>
    public interface IClaimsRepository
    {
        /// <summary>
        /// Returns all claims stored in the underlying data store.
        /// </summary>
        Task<IEnumerable<Claim>> GetAllAsync();

        /// <summary>
        /// Returns the claim with the supplied identifier, or <c>null</c> when not found.
        /// </summary>
        /// <param name="id">The unique identifier of the claim.</param>
        Task<Claim?> GetByIdAsync(string id);

        /// <summary>
        /// Persists a new <paramref name="claim"/>.
        /// </summary>
        Task AddAsync(Claim claim);

        /// <summary>
        /// Removes the claim with the supplied identifier when it exists.
        /// </summary>
        /// <param name="id">The unique identifier of the claim to remove.</param>
        Task DeleteAsync(string id);
    }
}
