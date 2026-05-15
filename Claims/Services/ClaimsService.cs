using Claims.Repositories;
using Claims.Validation;

namespace Claims.Services
{
   
    public class ClaimsService : IClaimsService
    {
        private readonly IClaimsRepository _claimsRepository;
        private readonly IAuditService _auditService;
        private readonly IClaimValidator _claimValidator;

      
        public ClaimsService(
            IClaimsRepository claimsRepository,
            IAuditService auditService,
            IClaimValidator claimValidator)
        {
            _claimsRepository = claimsRepository;
            _auditService = auditService;
            _claimValidator = claimValidator;
        }

       
        public Task<IEnumerable<Claim>> GetAllAsync() => _claimsRepository.GetAllAsync();

        public Task<Claim?> GetByIdAsync(string id) => _claimsRepository.GetByIdAsync(id);

        public async Task<Claim> CreateAsync(Claim claim)
        {
            await _claimValidator.ValidateAsync(claim);
            claim.Id = Guid.NewGuid().ToString();
            await _claimsRepository.AddAsync(claim);
            await _auditService.AuditClaimAsync(claim.Id, "POST");
            return claim;
        }

        public async Task DeleteAsync(string id)
        {
            await _auditService.AuditClaimAsync(id, "DELETE");
            await _claimsRepository.DeleteAsync(id);
        }
    }
}
