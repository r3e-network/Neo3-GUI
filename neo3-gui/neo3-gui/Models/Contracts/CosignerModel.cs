using System.Text.Json.Serialization;
using Neo.Network.P2P.Payloads;

namespace Neo.Models.Contracts
{
    /// <summary>
    /// Represents a transaction co-signer with witness scope
    /// </summary>
    public class CosignerModel
    {
        /// <summary>
        /// Account address of the co-signer
        /// </summary>
        public UInt160? Account { get; set; }

        /// <summary>
        /// Witness scope defining signature validity
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WitnessScope Scopes { get; set; } = WitnessScope.Global;
    }
}
