using Claims.Services;
using Claims.Tests.OtherUnitTests;
using Claims.Validation;
using Xunit;

namespace Claims.Tests
{
    public class CoversServiceTests
    {
        private readonly InMemoryCoversRepository _covers = new();
        private readonly RecordingAuditService _audit = new();
        private readonly StubPremiumCalculator _premium = new() { Result = 1234m };
        private readonly CoversService _sut;

        public CoversServiceTests()
        {
            _sut = new CoversService(_covers, _premium, _audit, new CoverValidator());
        }

        [Fact]
        public async Task CreateAsync_ValidCover_AssignsIdComputesPremiumAndAudits()
        {
            var start = DateTime.UtcNow.Date.AddDays(1);
            var cover = new Cover
            {
                StartDate = start,
                EndDate = start.AddMonths(3),
                Type = CoverType.Yacht
            };

            var created = await _sut.CreateAsync(cover);

            Assert.False(string.IsNullOrEmpty(created.Id));
            Assert.Equal(1234m, created.Premium);
            Assert.NotNull(await _covers.GetByIdAsync(created.Id));
            Assert.Contains(_audit.Entries, e => e.Id == created.Id && e.Method == "POST" && e.Entity == "Cover");
        }

        [Fact]
        public async Task CreateAsync_InvalidCover_DoesNotPersistOrAudit()
        {
            var cover = new Cover
            {
                StartDate = DateTime.UtcNow.Date.AddDays(-1),
                EndDate = DateTime.UtcNow.Date.AddDays(10),
                Type = CoverType.Yacht
            };

            await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(cover));

            Assert.Empty(await _covers.GetAllAsync());
            Assert.Empty(_audit.Entries);
        }

        [Fact]
        public async Task DeleteAsync_RemovesCoverAndAudits()
        {
            var start = DateTime.UtcNow.Date.AddDays(1);
            var created = await _sut.CreateAsync(new Cover
            {
                StartDate = start,
                EndDate = start.AddMonths(2),
                Type = CoverType.Tanker
            });
            _audit.Entries.Clear();

            await _sut.DeleteAsync(created.Id);

            Assert.Null(await _covers.GetByIdAsync(created.Id));
            Assert.Contains(_audit.Entries, e => e.Id == created.Id && e.Method == "DELETE" && e.Entity == "Cover");
        }

        [Fact]
        public void ComputePremium_DelegatesToCalculator()
        {
            _premium.Result = 999m;
            var result = _sut.ComputePremium(new DateTime(2025, 1, 1), new DateTime(2025, 2, 1), CoverType.Yacht);
            Assert.Equal(999m, result);
        }
    }
}
