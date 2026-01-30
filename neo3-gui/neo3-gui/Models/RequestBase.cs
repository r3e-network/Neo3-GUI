namespace Neo.Models
{
    /// <summary>
    /// Base request model with validation
    /// </summary>
    public abstract class RequestBase
    {
        public abstract bool IsValid(out string error);
    }
}
