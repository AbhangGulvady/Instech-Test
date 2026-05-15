using Claims.Repositories;
using Claims.Validation;

namespace Claims.Services
{
    /// <summary>
    /// Default <see cref="ICoversService"/> implementation that coordinates the
    /// covers repository, the premium calculator, the cover validator and the
    /// audit service.
    /// </summary>
    public class CoversService : ICoversService
    {
        private readonly ICoversRepository _coversRepository;
        private readonly IPremiumCalculator _premiumCalculator;
        private readonly IAuditService _auditService;
        private readonly ICoverValidator _coverValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoversService"/> class.
        /// </summary>
        public CoversService(
            ICoversRepository coversRepository,
            IPremiumCalculator premiumCalculator,
            IAuditService auditService,
            ICoverValidator coverValidator)
        {
            _coversRepository = coversRepository;
            _premiumCalculator = premiumCalculator;
            _auditService = auditService;
            _coverValidator = coverValidator;
        }

        /// <inheritdoc />
        public Task<IEnumerable<Cover>> GetAllAsync() => _coversRepository.GetAllAsync();

        /// <inheritdoc />
        public Task<Cover?> GetByIdAsync(string id) => _coversRepository.GetByIdAsync(id);

        /// <inheritdoc />
        public async Task<Cover> CreateAsync(Cover cover)
        {
            _coverValidator.Validate(cover);
            cover.Id = Guid.NewGuid().ToString();
            cover.Premium = _premiumCalculator.ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
            await _coversRepository.AddAsync(cover);
            await _auditService.AuditCoverAsync(cover.Id, "POST");
            return cover;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(string id)
        {
            await _auditService.AuditCoverAsync(id, "DELETE");
            await _coversRepository.DeleteAsync(id);
        }

        /// <inheritdoc />
        public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
            => _premiumCalculator.ComputePremium(startDate, endDate, coverType);
    }
}
