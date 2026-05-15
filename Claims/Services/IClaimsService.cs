namespace Claims.Services
{
    /// <summary>
    /// Application service that orchestrates claim related use cases such as
    /// retrieval, creation and deletion, while ensuring auditing happens.
    /// </summary>
    public interface IClaimsService
    {
        /// <summary>Returns every claim known to the system.</summary>
        Task<IEnumerable<Claim>> GetAllAsync();

        /// <summary>Returns the claim with the supplied identifier, or <c>null</c>.</summary>
        /// <param name="id">The unique identifier of the claim.</param>
        Task<Claim?> GetByIdAsync(string id);

        /// <summary>
        /// Creates a new claim, assigning it a fresh identifier and writing an audit entry.
        /// </summary>
        /// <param name="claim">The claim to create.</param>
        /// <returns>The persisted claim, including its generated identifier.</returns>
        Task<Claim> CreateAsync(Claim claim);

        /// <summary>
        /// Deletes the claim with the supplied identifier and writes an audit entry.
        /// </summary>
        /// <param name="id">The identifier of the claim to delete.</param>
        Task DeleteAsync(string id);
    }
}
