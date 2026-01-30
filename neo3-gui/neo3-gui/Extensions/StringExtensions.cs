namespace Neo.Extensions
{
    /// <summary>
    /// String extension methods
    /// </summary>
    public static class StringExtensions
    {
        public static bool IsNull(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        public static bool NotNull(this string text)
        {
            return !IsNull(text);
        }

        public static bool ToBool(this string input)
        {
            if (input == null) return false;
            input = input.ToLowerInvariant();
            return input == "true" || input == "yes" || input == "1";
        }
    }
}
