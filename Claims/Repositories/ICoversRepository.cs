namespace Claims.Repositories
{
    /// <summary>
    /// Provides persistence operations for <see cref="Cover"/> entities.
    /// </summary>
    public interface ICoversRepository
    {
        /// <summary>
        /// Returns all covers stored in the underlying data store.
        /// </summary>
        Task<IEnumerable<Cover>> GetAllAsync();

        /// <summary>
        /// Returns the cover with the supplied identifier, or <c>null</c> when not found.
        /// </summary>
        /// <param name="id">The unique identifier of the cover.</param>
        Task<Cover?> GetByIdAsync(string id);

        /// <summary>
        /// Persists a new <paramref name="cover"/>.
        /// </summary>
        Task AddAsync(Cover cover);

        /// <summary>
        /// Removes the cover with the supplied identifier when it exists.
        /// </summary>
        /// <param name="id">The unique identifier of the cover to remove.</param>
        Task DeleteAsync(string id);
    }
}
