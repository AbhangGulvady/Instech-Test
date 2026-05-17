using Claims.Repositories;
using Claims.Validation;

namespace Claims.Services
{
    public class CoversService : ICoversService
    {
        private readonly ICoversRepository _coversRepository;
        private readonly IPremiumCalculator _premiumCalculator;
        private readonly IAuditService _auditService;
        private readonly ICoverValidator _coverValidator;

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

        public Task<IEnumerable<Cover>> GetAllAsync() => _coversRepository.GetAllAsync();

        public Task<Cover?> GetByIdAsync(string id) => _coversRepository.GetByIdAsync(id);

        public async Task<Cover> CreateAsync(Cover cover)
        {
            _coverValidator.Validate(cover);
            cover.Id = Guid.NewGuid().ToString();
            cover.Premium = _premiumCalculator.ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
            await _coversRepository.AddAsync(cover);
            await _auditService.AuditCoverAsync(cover.Id, "POST");
            return cover;
        }

        public async Task DeleteAsync(string id)
        {
            await _auditService.AuditCoverAsync(id, "DELETE");
            await _coversRepository.DeleteAsync(id);
        }

        public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            return _premiumCalculator.ComputePremium(startDate, endDate, coverType);
        }
            
    }
}
