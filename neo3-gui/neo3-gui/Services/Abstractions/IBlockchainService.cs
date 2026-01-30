using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;

namespace Neo.Services.Abstractions
{
    /// <summary>
    /// Blockchain data access service interface
    /// </summary>
    public interface IBlockchainService
    {
        DataCache GetSnapshot();
        uint GetCurrentHeight();
        uint GetCurrentHeaderHeight();
        Block GetBlock(uint index);
        Block GetBlock(UInt256 hash);
        Header GetHeader(uint index);
        Header GetHeader(UInt256 hash);
        Transaction GetTransaction(UInt256 hash);
        TransactionState GetTransactionState(UInt256 hash);
        ContractState GetContract(UInt160 hash);
    }
}
