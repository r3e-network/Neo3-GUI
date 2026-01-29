using System;
using Neo.Network.P2P.Payloads;

namespace Neo.Models.Blocks
{
    /// <summary>
    /// Lightweight block preview model
    /// </summary>
    public class BlockPreviewModel
    {
        /// <summary>
        /// Creates a BlockPreviewModel from a Block
        /// </summary>
        public BlockPreviewModel(Block block)
        {
            BlockHash = block.Hash;
            BlockHeight = block.Index;
            Timestamp = block.Timestamp;
            TransactionCount = block.Transactions?.Length ?? 0;
            Size = block.Size;
        }

        /// <summary>Block hash</summary>
        public UInt256? BlockHash { get; set; }

        /// <summary>Block height</summary>
        public uint BlockHeight { get; set; }

        /// <summary>Block time (local)</summary>
        public DateTime BlockTime => Timestamp.FromTimestampMS().ToLocalTime();

        /// <summary>Timestamp (ms)</summary>
        public ulong Timestamp { get; set; }

        /// <summary>Block size in bytes</summary>
        public int Size { get; set; }

        /// <summary>Transaction count</summary>
        public int TransactionCount { get; set; }
    }
}
