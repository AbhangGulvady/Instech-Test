namespace Claims.Services
{
    
    public class PremiumCalculator : IPremiumCalculator
    {
        /// constant base daily rate for premium calculation.
        public const decimal BaseDailyRate = 1250m;

        private const int FirstPeriodDays = 30;
        private const int MidPeriodDays = 150;
        private const int MidPeriodEndDay = FirstPeriodDays + MidPeriodDays; // 180

        
        public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            var totalDays = (int)(endDate - startDate).TotalDays;
            if (totalDays <= 0)
            {
                return Decimal.Zero;
            }

            // Discount is adjusted.
            var premiumPerDay = BaseDailyRate * GetCoverTypeMultiplier(coverType);
            var midDiscount = coverType == CoverType.Yacht ? 0.05m : 0.02m;
            var lateDiscount = coverType == CoverType.Yacht ? 0.08m : 0.03m;

            var firstDays = Math.Min(totalDays, FirstPeriodDays);
            var midDays = Math.Min(Math.Max(totalDays - FirstPeriodDays, 0), MidPeriodDays);
            var lateDays = Math.Max(totalDays - MidPeriodEndDay, 0);

            return firstDays * premiumPerDay
                 + midDays * premiumPerDay * (1m - midDiscount)
                 + lateDays * premiumPerDay * (1m - lateDiscount);
        }

        

        private static decimal GetCoverTypeMultiplier(CoverType coverType) => coverType switch
        {
            CoverType.Yacht => 1.1m, // 1m + 0.1m for yachts (10% increase)
            CoverType.PassengerShip => 1.2m, // 1m + 0.2m for passenger ships (20% increase)
            CoverType.Tanker => 1.5m, // 1m + 0.5m for tankers (50% increase)
            _ => 1.3m // 1m + 0.3m for other cover types (30% increase)
        };
    }
}
