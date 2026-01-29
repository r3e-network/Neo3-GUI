using Neo.SmartContract.Manifest;

namespace Neo.Models.Contracts
{
    /// <summary>
    /// Contract manifest model
    /// </summary>
    public class ManifestModel
    {
        /// <summary>
        /// Creates a ManifestModel from a ContractManifest
        /// </summary>
        public ManifestModel(ContractManifest? manifest)
        {
            if (manifest != null)
            {
                ContractName = manifest.Name;
                SupportedStandards = manifest.SupportedStandards;
                Groups = manifest.Groups;
                Permissions = manifest.Permissions;
                Trusts = manifest.Trusts;
                Abi = new ContractAbiModel(manifest.Abi);
                Extra = manifest.Extra;
            }
        }

        /// <summary>
        /// Contract name
        /// </summary>
        public string? ContractName { get; set; }

        /// <summary>
        /// Mutually trusted contract groups
        /// </summary>
        public ContractGroup[]? Groups { get; set; }

        /// <summary>
        /// Contract ABI definition
        /// </summary>
        public ContractAbiModel? Abi { get; set; }

        /// <summary>
        /// Invocation permissions
        /// </summary>
        public ContractPermission[]? Permissions { get; set; }

        /// <summary>
        /// Trusted contracts or groups
        /// </summary>
        public WildcardContainer<ContractPermissionDescriptor>? Trusts { get; set; }

        /// <summary>
        /// Supported NEP standards
        /// </summary>
        public string[]? SupportedStandards { get; set; }

        /// <summary>
        /// Custom user data
        /// </summary>
        public object? Extra { get; set; }
    }
}
