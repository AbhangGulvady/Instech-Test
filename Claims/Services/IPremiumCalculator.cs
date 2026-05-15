namespace Claims.Services
{
    /// <summary>
    /// Calculates the premium amount charged for an insurance cover.
    /// </summary>
    public interface IPremiumCalculator
    {
        /// <summary>
        /// Computes the total premium for a cover spanning <paramref name="startDate"/>
        /// to <paramref name="endDate"/> of the supplied <paramref name="coverType"/>.
        /// </summary>
        /// <param name="startDate">Inclusive start date of the cover.</param>
        /// <param name="endDate">Exclusive end date of the cover.</param>
        /// <param name="coverType">The kind of vessel being insured.</param>
        /// <returns>The total premium amount.</returns>
        decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType);
    }
}
