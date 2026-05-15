using Claims.Services;
using Claims.Tests.OtherUnitTests;
using Claims.Validation;
using Xunit;

namespace Claims.Tests
{
    public class ClaimsServiceTests
    {
        private readonly InMemoryClaimsRepository _claims = new();
        private readonly InMemoryCoversRepository _covers = new();
        private readonly RecordingAuditService _audit = new();
        private readonly ClaimsService _sut;

        public ClaimsServiceTests()
        {
            var validator = new ClaimValidator(_covers);
            _sut = new ClaimsService(_claims, _audit, validator);
        }

        private Cover SeedValidCover()
        {
            var cover = new Cover
            {
                Id = Guid.NewGuid().ToString(),
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 6, 1),
                Type = CoverType.Yacht
            };
            _covers.Seed(cover);
            return cover;
        }

        [Fact]
        public async Task CreateAsync_ValidClaim_PersistsAndAudits()
        {
            var cover = SeedValidCover();
            var claim = new Claim
            {
                CoverId = cover.Id,
                Created = new DateTime(2025, 2, 1),
                DamageCost = 1000m,
                Name = "Test"
            };

            var created = await _sut.CreateAsync(claim);

            Assert.False(string.IsNullOrEmpty(created.Id));
            Assert.NotNull(await _claims.GetByIdAsync(created.Id));
            Assert.Contains(_audit.Entries, e => e.Id == created.Id && e.Method == "POST" && e.Entity == "Claim");
        }

        [Fact]
        public async Task CreateAsync_InvalidClaim_DoesNotPersistOrAudit()
        {
            var cover = SeedValidCover();
            var claim = new Claim
            {
                CoverId = cover.Id,
                Created = new DateTime(2025, 2, 1),
                DamageCost = 1_000_000m
            };

            await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(claim));

            Assert.Empty(await _claims.GetAllAsync());
            Assert.Empty(_audit.Entries);
        }

        [Fact]
        public async Task DeleteAsync_RemovesClaimAndAudits()
        {
            var cover = SeedValidCover();
            var claim = new Claim
            {
                CoverId = cover.Id,
                Created = new DateTime(2025, 2, 1),
                DamageCost = 1000m
            };
            var created = await _sut.CreateAsync(claim);
            _audit.Entries.Clear();

            await _sut.DeleteAsync(created.Id);

            Assert.Null(await _claims.GetByIdAsync(created.Id));
            Assert.Contains(_audit.Entries, e => e.Id == created.Id && e.Method == "DELETE" && e.Entity == "Claim");
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllPersistedClaims()
        {
            var cover = SeedValidCover();
            await _sut.CreateAsync(new Claim { CoverId = cover.Id, Created = new DateTime(2025, 2, 1), DamageCost = 1m });
            await _sut.CreateAsync(new Claim { CoverId = cover.Id, Created = new DateTime(2025, 3, 1), DamageCost = 2m });

            var all = await _sut.GetAllAsync();

            Assert.Equal(2, all.Count());
        }
    }
}
