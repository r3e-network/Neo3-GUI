using Neo.Network.P2P.Payloads;

namespace Neo.Services.Abstractions
{
    /// <summary>
    /// Transaction broadcast service interface
    /// </summary>
    public interface ITransactionBroadcastService
    {
        Task BroadcastAsync(Transaction tx);
    }
}
