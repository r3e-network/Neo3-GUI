namespace Neo.Services.Abstractions
{
    /// <summary>
    /// Address validation service interface
    /// </summary>
    public interface IAddressValidator
    {
        bool IsValid(string address);
        UInt160 Parse(string address);
    }
}
