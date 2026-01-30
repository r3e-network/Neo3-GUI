using Neo.Common;
using Neo.Network.P2P.Payloads;
using Neo.Services.Abstractions;
using Neo.SmartContract;
using Neo.Wallets;

namespace Neo.Services.Core
{
    /// <summary>
    /// Wallet operations service implementation
    /// </summary>
    public class WalletService : IWalletService
    {
        private readonly IBlockchainService _blockchain;

        public WalletService(IBlockchainService blockchain)
        {
            _blockchain = blockchain;
        }

        public Wallet CurrentWallet => Program.Starter.CurrentWallet;

        public bool IsWalletOpen => CurrentWallet != null;

        public Transaction CreateTransaction(
            byte[] script, 
            UInt160 sender, 
            params Signer[] signers)
        {
            return CurrentWallet.MakeTransaction(
                _blockchain.GetSnapshot(), 
                script, 
                sender, 
                signers, 
                maxGas: Constant.TestMode);
        }

        public (bool success, ContractParametersContext context) SignTransaction(
            Transaction tx)
        {
            var snapshot = _blockchain.GetSnapshot();
            var context = new ContractParametersContext(
                snapshot, 
                tx, 
                CliSettings.Default.Protocol.Network);
            
            CurrentWallet.Sign(context);
            
            if (context.Completed)
            {
                tx.Witnesses = context.GetWitnesses();
            }
            
            return (context.Completed, context);
        }
    }
}
