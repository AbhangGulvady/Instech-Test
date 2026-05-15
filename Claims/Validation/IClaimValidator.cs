namespace Claims.Validation
{
    
    public interface IClaimValidator
    {
        
        Task ValidateAsync(Claim claim);
    }
}
