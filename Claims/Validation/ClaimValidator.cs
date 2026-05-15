using Claims.Repositories;

namespace Claims.Validation
{
    public class ClaimValidator : IClaimValidator
    {
        public const decimal MaxDamageCost = 100_000m;

        private readonly ICoversRepository _coversRepository;

        public ClaimValidator(ICoversRepository coversRepository)
        {
            _coversRepository = coversRepository;
        }

        public async Task ValidateAsync(Claim claim)
        {
            if (claim is null)
            {
                throw new ValidationException("Claim must be provided.");
            }

            if (claim.DamageCost > MaxDamageCost)
            {
                throw new ValidationException($"Claim DamageCost cannot exceed {MaxDamageCost}.");
            }

            if (string.IsNullOrWhiteSpace(claim.CoverId))
            {
                throw new ValidationException("Claim must reference a Cover.");
            }

            var cover = await _coversRepository.GetByIdAsync(claim.CoverId);
            if (cover is null)
            {
                throw new ValidationException($"Related Cover '{claim.CoverId}' does not exist.");
            }

            if (claim.Created.Date < cover.StartDate.Date || claim.Created.Date > cover.EndDate.Date)
            {
                throw new ValidationException("Claim Created date must be within the period of the related Cover.");
            }
        }
    }
}
