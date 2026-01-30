namespace Neo.Events
{
    /// <summary>
    /// Block synced event
    /// </summary>
    public record BlockSyncedEvent(uint Height, UInt256 Hash);

    /// <summary>
    /// Transaction confirmed event
    /// </summary>
    public record TransactionConfirmedEvent(UInt256 TxId);

    /// <summary>
    /// Wallet balance changed event
    /// </summary>
    public record BalanceChangedEvent(UInt160 Address, UInt160 Asset);
}
