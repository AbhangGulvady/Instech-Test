namespace Claims.Validation
{
    
    public class CoverValidator : ICoverValidator
    {
        
        public void Validate(Cover cover)
        {
            if (cover is null)
            {
                throw new ValidationException("Cover must be provided.");
            }

            var today = DateTime.UtcNow.Date;
            if (cover.StartDate.Date < today)
            {
                throw new ValidationException("Cover StartDate cannot be in the past.");
            }

            if (cover.EndDate.Date < cover.StartDate.Date)
            {
                throw new ValidationException("Cover EndDate cannot be earlier than StartDate.");
            }

            var maxEndDate = cover.StartDate.AddYears(1);
            if (cover.EndDate > maxEndDate)
            {
                throw new ValidationException("Total insurance period cannot exceed 1 year.");
            }
        }
    }
}
