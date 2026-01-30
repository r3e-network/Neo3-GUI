using Neo.Common.Utility;
using Neo.Network.P2P.Payloads;
using Neo.Services.Abstractions;

namespace Neo.Services.Core
{
    /// <summary>
    /// Transaction broadcast service implementation
    /// </summary>
    public class TransactionBroadcastService : ITransactionBroadcastService
    {
        public async Task BroadcastAsync(Transaction tx)
        {
            Program.Starter.NeoSystem.Blockchain.Tell(tx);
            await Task.Run(() => UnconfirmedTransactionCache.AddTransaction(tx));
        }
    }
}
