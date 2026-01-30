namespace Neo.Extensions
{
    /// <summary>
    /// Enum extension methods
    /// </summary>
    public static class EnumExtensions
    {
        public static T ToEnum<T>(this string text, bool ignoreCase = false) 
            where T : unmanaged, Enum
        {
            T result = Enum.Parse<T>(text, ignoreCase);
            if (!Enum.IsDefined(result)) 
                throw new InvalidCastException();
            return result;
        }

        public static T AsEnum<T>(this string text, T defaultValue = default, 
            bool ignoreCase = false) where T : unmanaged, Enum
        {
            try
            {
                return Enum.Parse<T>(text, ignoreCase);
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
