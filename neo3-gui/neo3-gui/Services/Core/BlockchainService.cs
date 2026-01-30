using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.Services.Abstractions;
using Neo.SmartContract;
using Neo.SmartContract.Native;

namespace Neo.Services.Core
{
    /// <summary>
    /// Blockchain data access service implementation
    /// </summary>
    public class BlockchainService : IBlockchainService
    {
        public DataCache GetSnapshot()
        {
            return Program.Starter.NeoSystem.StoreView;
        }

        public uint GetCurrentHeight()
        {
            return NativeContract.Ledger.CurrentIndex(GetSnapshot());
        }

        public uint GetCurrentHeaderHeight()
        {
            return Program.Starter.NeoSystem.HeaderCache.Last?.Index 
                ?? GetCurrentHeight();
        }

        public Block GetBlock(uint index)
        {
            return NativeContract.Ledger.GetBlock(GetSnapshot(), index);
        }

        public Block GetBlock(UInt256 hash)
        {
            return NativeContract.Ledger.GetBlock(GetSnapshot(), hash);
        }

        public Header GetHeader(uint index)
        {
            return NativeContract.Ledger.GetHeader(GetSnapshot(), index);
        }

        public Header GetHeader(UInt256 hash)
        {
            return NativeContract.Ledger.GetHeader(GetSnapshot(), hash);
        }

        public Transaction GetTransaction(UInt256 hash)
        {
            return NativeContract.Ledger.GetTransaction(GetSnapshot(), hash);
        }

        public TransactionState GetTransactionState(UInt256 hash)
        {
            return NativeContract.Ledger.GetTransactionState(GetSnapshot(), hash);
        }

        public ContractState GetContract(UInt160 hash)
        {
            return NativeContract.ContractManagement.GetContract(GetSnapshot(), hash);
        }
    }
}
