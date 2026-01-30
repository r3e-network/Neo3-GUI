using Neo.Network.P2P.Payloads;
using Neo.Services.Abstractions;
using Neo.SmartContract;
using Neo.VM;
using Neo.Wallets;

namespace Neo.Services.Core
{
    /// <summary>
    /// Transaction factory implementation
    /// </summary>
    public class TransactionFactory : ITransactionFactory
    {
        private readonly IBlockchainService _blockchain;
        private readonly IWalletService _wallet;

        public TransactionFactory(
            IBlockchainService blockchain,
            IWalletService wallet)
        {
            _blockchain = blockchain;
            _wallet = wallet;
        }

        public Transaction CreateTransfer(
            UInt160 from,
            UInt160 to,
            UInt160 asset,
            BigDecimal amount)
        {
            var output = new TransferOutput
            {
                AssetId = asset,
                Value = amount,
                ScriptHash = to
            };

            return _wallet.CurrentWallet.MakeTransaction(
                _blockchain.GetSnapshot(),
                new[] { output },
                from);
        }
    }
}
