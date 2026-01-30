namespace Neo.Common.Validation
{
    /// <summary>
    /// Request validation result
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; }
        public string Error { get; }

        private ValidationResult(bool valid, string error = null)
        {
            IsValid = valid;
            Error = error;
        }

        public static ValidationResult Success() => new(true);
        public static ValidationResult Fail(string error) => new(false, error);
    }
}
