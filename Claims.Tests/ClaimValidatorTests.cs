using Claims.Tests.OtherUnitTests;
using Claims.Validation;
using Xunit;

namespace Claims.Tests
{
    public class ClaimValidatorTests
    {
        private readonly InMemoryCoversRepository _covers = new();
        private readonly ClaimValidator _sut;

        public ClaimValidatorTests()
        {
            _sut = new ClaimValidator(_covers);
        }

        private Cover SeedCover(DateTime start, DateTime end)
        {
            var cover = new Cover
            {
                Id = Guid.NewGuid().ToString(),
                StartDate = start,
                EndDate = end,
                Type = CoverType.Yacht
            };
            _covers.Seed(cover);
            return cover;
        }

        [Fact]
        public async Task Validate_DamageCostExceedsLimit_Throws()
        {
            var cover = SeedCover(new DateTime(2025, 1, 1), new DateTime(2025, 6, 1));
            var claim = new Claim
            {
                CoverId = cover.Id,
                Created = new DateTime(2025, 2, 1),
                DamageCost = 100_001m
            };

            var ex = await Assert.ThrowsAsync<ValidationException>(() => _sut.ValidateAsync(claim));
            Assert.Contains("DamageCost", ex.Message);
        }

        [Fact]
        public async Task Validate_CreatedBeforeCoverStart_Throws()
        {
            var cover = SeedCover(new DateTime(2025, 1, 1), new DateTime(2025, 6, 1));
            var claim = new Claim
            {
                CoverId = cover.Id,
                Created = new DateTime(2024, 12, 31),
                DamageCost = 1m
            };

            await Assert.ThrowsAsync<ValidationException>(() => _sut.ValidateAsync(claim));
        }

        [Fact]
        public async Task Validate_CreatedAfterCoverEnd_Throws()
        {
            var cover = SeedCover(new DateTime(2025, 1, 1), new DateTime(2025, 6, 1));
            var claim = new Claim
            {
                CoverId = cover.Id,
                Created = new DateTime(2025, 6, 2),
                DamageCost = 1m
            };

            await Assert.ThrowsAsync<ValidationException>(() => _sut.ValidateAsync(claim));
        }

        [Fact]
        public async Task Validate_UnknownCover_Throws()
        {
            var claim = new Claim
            {
                CoverId = "missing",
                Created = new DateTime(2025, 1, 1),
                DamageCost = 1m
            };

            await Assert.ThrowsAsync<ValidationException>(() => _sut.ValidateAsync(claim));
        }

        [Fact]
        public async Task Validate_ValidClaim_DoesNotThrow()
        {
            var cover = SeedCover(new DateTime(2025, 1, 1), new DateTime(2025, 6, 1));
            var claim = new Claim
            {
                CoverId = cover.Id,
                Created = new DateTime(2025, 3, 1),
                DamageCost = 50_000m
            };

            await _sut.ValidateAsync(claim);
        }

        [Fact]
        public async Task Validate_MissingCoverId_Throws()
        {
            var claim = new Claim
            {
                CoverId = "",
                Created = new DateTime(2025, 1, 1),
                DamageCost = 1m
            };

            await Assert.ThrowsAsync<ValidationException>(() => _sut.ValidateAsync(claim));
        }
    }
}
