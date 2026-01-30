using Neo.Exceptions;

namespace Neo.Common.Validation
{
    /// <summary>
    /// Parameter validation helper
    /// </summary>
    public static class Guard
    {
        public static void NotNull(object value, string name)
        {
            if (value == null)
                throw new ParameterNullException(name);
        }

        public static void NotNullOrEmpty(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ParameterNullException(name);
        }

        public static void Positive(int value, string name)
        {
            if (value <= 0)
                throw new ValidationException(name, 
                    $"{name} must be positive");
        }
    }
}
