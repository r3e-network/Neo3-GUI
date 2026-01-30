namespace Neo.Common.Validation
{
    /// <summary>
    /// Transfer request validator
    /// </summary>
    public static class TransferValidator
    {
        public static ValidationResult Validate(
            UInt160 receiver, 
            string amount, 
            string asset)
        {
            if (receiver == null)
                return ValidationResult.Fail("receiver is required");
            
            if (string.IsNullOrWhiteSpace(amount))
                return ValidationResult.Fail("amount is required");
            
            if (string.IsNullOrWhiteSpace(asset))
                return ValidationResult.Fail("asset is required");
            
            return ValidationResult.Success();
        }
    }
}
