using Neo.Network.P2P.Payloads;

namespace Neo.Services.Abstractions
{
    /// <summary>
    /// Transaction factory interface
    /// </summary>
    public interface ITransactionFactory
    {
        Transaction CreateTransfer(
            UInt160 from, 
            UInt160 to, 
            UInt160 asset, 
            BigDecimal amount);
    }
}
