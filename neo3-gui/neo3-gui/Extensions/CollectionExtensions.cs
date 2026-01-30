namespace Neo.Extensions
{
    /// <summary>
    /// Collection extension methods
    /// </summary>
    public static class CollectionExtensions
    {
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        public static bool NotEmpty<T>(this IEnumerable<T> source)
        {
            return !source.IsEmpty();
        }
    }
}
