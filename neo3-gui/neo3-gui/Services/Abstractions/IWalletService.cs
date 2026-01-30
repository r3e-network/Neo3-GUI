using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.Wallets;

namespace Neo.Services.Abstractions
{
    /// <summary>
    /// Wallet operations service interface
    /// </summary>
    public interface IWalletService
    {
        Wallet CurrentWallet { get; }
        bool IsWalletOpen { get; }
        Transaction CreateTransaction(byte[] script, UInt160 sender, params Signer[] signers);
        (bool success, ContractParametersContext context) SignTransaction(Transaction tx);
    }
}
