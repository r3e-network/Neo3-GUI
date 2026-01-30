using Neo.Common;
using Neo.Services.Abstractions;

namespace Neo.Services.Core
{
    /// <summary>
    /// Address validation service implementation
    /// </summary>
    public class AddressValidator : IAddressValidator
    {
        public bool IsValid(string address)
        {
            try
            {
                Parse(address);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public UInt160 Parse(string address)
        {
            return address.ToScriptHash(CliSettings.Default.Protocol.AddressVersion);
        }
    }
}
