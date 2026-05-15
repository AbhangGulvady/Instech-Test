using Claims.Services;
using Xunit;

namespace Claims.Tests
{
    public class PremiumCalculatorTests
    {
        private readonly PremiumCalculator _sut = new();

        [Fact]
        public void ComputePremium_SameDay_ReturnsZero()
        {
            var date = new DateTime(2025, 1, 1);
            Assert.Equal(0m, _sut.ComputePremium(date, date, CoverType.Yacht));
        }

        [Fact]
        public void ComputePremium_NegativePeriod_ReturnsZero()
        {
            var start = new DateTime(2025, 1, 10);
            Assert.Equal(0m, _sut.ComputePremium(start, start.AddDays(-1), CoverType.Yacht));
        }

        [Theory]
        [InlineData(CoverType.Yacht, 1.1)]
        [InlineData(CoverType.PassengerShip, 1.2)]
        [InlineData(CoverType.Tanker, 1.5)]
        [InlineData(CoverType.ContainerShip, 1.3)]
        [InlineData(CoverType.BulkCarrier, 1.3)]
        public void ComputePremium_OneDay_AppliesCoverTypeMultiplierAtFullRate(CoverType type, double multiplier)
        {
            var start = new DateTime(2025, 1, 1);

            var premium = _sut.ComputePremium(start, start.AddDays(1), type);

            Assert.Equal(1250m * (decimal)multiplier, premium);
        }

        [Theory]
        [InlineData(CoverType.Yacht, 1.1)]
        [InlineData(CoverType.PassengerShip, 1.2)]
        [InlineData(CoverType.Tanker, 1.5)]
        [InlineData(CoverType.ContainerShip, 1.3)]
        [InlineData(CoverType.BulkCarrier, 1.3)]
        public void ComputePremium_ThirtyDays_AllAtFullRate(CoverType type, double multiplier)
        {
            var start = new DateTime(2025, 1, 1);

            var premium = _sut.ComputePremium(start, start.AddDays(30), type);

            Assert.Equal(30m * 1250m * (decimal)multiplier, premium);
        }

        [Fact]
        public void ComputePremium_HundredDays_AppliesMidPeriodDiscountForYacht()
        {
            var start = new DateTime(2025, 1, 1);
            var perDay = 1250m * 1.1m;

            var premium = _sut.ComputePremium(start, start.AddDays(100), CoverType.Yacht);

            // 30 days at full rate + 70 days at 95%.
            var expected = 30m * perDay + 70m * perDay * 0.95m;
            Assert.Equal(expected, premium);
        }

        [Fact]
        public void ComputePremium_HundredDays_AppliesMidPeriodDiscountForOtherTypes()
        {
            var start = new DateTime(2025, 1, 1);
            var perDay = 1250m * 1.5m;

            var premium = _sut.ComputePremium(start, start.AddDays(100), CoverType.Tanker);

            // 30 days at full rate + 70 days at 98%.
            var expected = 30m * perDay + 70m * perDay * 0.98m;
            Assert.Equal(expected, premium);
        }

        [Fact]
        public void ComputePremium_OneYear_AppliesAllThreeBracketsForYacht()
        {
            var start = new DateTime(2025, 1, 1);
            var perDay = 1250m * 1.1m;

            var premium = _sut.ComputePremium(start, start.AddDays(365), CoverType.Yacht);

            // 30 @ 100% + 150 @ 95% + 185 @ 92%.
            var expected = 30m * perDay
                         + 150m * perDay * 0.95m
                         + 185m * perDay * 0.92m;
            Assert.Equal(expected, premium);
        }

        [Fact]
        public void ComputePremium_OneYear_AppliesAllThreeBracketsForOtherTypes()
        {
            var start = new DateTime(2025, 1, 1);
            var perDay = 1250m * 1.3m;

            var premium = _sut.ComputePremium(start, start.AddDays(365), CoverType.ContainerShip);

            // 30 @ 100% + 150 @ 98% + 185 @ 97%.
            var expected = 30m * perDay
                         + 150m * perDay * 0.98m
                         + 185m * perDay * 0.97m;
            Assert.Equal(expected, premium);
        }

        [Fact]
        public void ComputePremium_LongerPeriod_IsGreaterThanShorterPeriod()
        {
            var start = new DateTime(2025, 1, 1);

            var shorter = _sut.ComputePremium(start, start.AddDays(10), CoverType.Yacht);
            var longer = _sut.ComputePremium(start, start.AddDays(60), CoverType.Yacht);

            Assert.True(longer > shorter);
        }
    }
}
