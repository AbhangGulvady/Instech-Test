using Claims.Validation;
using Xunit;

namespace Claims.Tests
{
    public class CoverValidatorTests
    {
        private readonly CoverValidator _sut = new();

        [Fact]
        public void Validate_StartDateInPast_Throws()
        {
            var cover = new Cover
            {
                StartDate = DateTime.UtcNow.Date.AddDays(-1),
                EndDate = DateTime.UtcNow.Date.AddDays(10),
                Type = CoverType.Yacht
            };

            var ex = Assert.Throws<ValidationException>(() => _sut.Validate(cover));
            Assert.Contains("StartDate", ex.Message);
        }

        [Fact]
        public void Validate_PeriodExceedsOneYear_Throws()
        {
            var start = DateTime.UtcNow.Date.AddDays(1);
            var cover = new Cover
            {
                StartDate = start,
                EndDate = start.AddYears(1).AddDays(1),
                Type = CoverType.Yacht
            };

            var ex = Assert.Throws<ValidationException>(() => _sut.Validate(cover));
            Assert.Contains("1 year", ex.Message);
        }

        [Fact]
        public void Validate_EndBeforeStart_Throws()
        {
            var start = DateTime.UtcNow.Date.AddDays(5);
            var cover = new Cover
            {
                StartDate = start,
                EndDate = start.AddDays(-1),
                Type = CoverType.Yacht
            };

            Assert.Throws<ValidationException>(() => _sut.Validate(cover));
        }

        [Fact]
        public void Validate_ValidCover_DoesNotThrow()
        {
            var start = DateTime.UtcNow.Date.AddDays(1);
            var cover = new Cover
            {
                StartDate = start,
                EndDate = start.AddMonths(6),
                Type = CoverType.Yacht
            };

            _sut.Validate(cover);
        }

        [Fact]
        public void Validate_NullCover_Throws()
        {
            Assert.Throws<ValidationException>(() => _sut.Validate(null!));
        }
    }
}
