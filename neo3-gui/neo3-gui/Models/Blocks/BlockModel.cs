using System;
using System.Linq;
using System.Numerics;
using Neo.Models.Transactions;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Native;

namespace Neo.Models.Blocks
{
    /// <summary>
    /// Block information model
    /// </summary>
    public class BlockModel
    {
        /// <summary>
        /// Creates a BlockModel from a Block
        /// </summary>
        public BlockModel(Block block)
        {
            BlockHash = block.Hash;
            BlockHeight = block.Index;
            Timestamp = block.Timestamp;
            Size = block.Size;
            Version = block.Version;
            PrevHash = block.PrevHash;
            MerkleRoot = block.MerkleRoot;
            NextConsensusHash = block.NextConsensus;
            Witness = new WitnessModel(block.Witness);
            PrimaryIndex = block.PrimaryIndex;
            if (block.Transactions.NotEmpty())
            {
                SystemFee = new BigDecimal((BigInteger)block.Transactions.Sum(t => t.SystemFee),
                    NativeContract.GAS.Decimals).ToString();
                NetworkFee = new BigDecimal((BigInteger)block.Transactions.Sum(t => t.NetworkFee),
                    NativeContract.GAS.Decimals).ToString();
            }
        }

        /// <summary>Block hash</summary>
        public UInt256? BlockHash { get; set; }

        /// <summary>Block height/index</summary>
        public uint BlockHeight { get; set; }

        /// <summary>Block time (local)</summary>
        public DateTime BlockTime => Timestamp.FromTimestampMS().ToLocalTime();

        /// <summary>Number of confirmations</summary>
        public uint Confirmations { get; set; }

        /// <summary>Merkle root hash</summary>
        public UInt256? MerkleRoot { get; set; }

        /// <summary>Total network fee</summary>
        public string? NetworkFee { get; set; }

        /// <summary>Next consensus address</summary>
        public string? NextConsensus => NextConsensusHash?.ToAddress();

        /// <summary>Next consensus hash</summary>
        public UInt160? NextConsensusHash { get; set; }

        /// <summary>Previous block hash</summary>
        public UInt256? PrevHash { get; set; }

        /// <summary>Primary validator index</summary>
        public byte PrimaryIndex { get; set; }

        /// <summary>Block size in bytes</summary>
        public int Size { get; set; }

        /// <summary>Total system fee</summary>
        public string? SystemFee { get; set; }

        /// <summary>Block timestamp (ms)</summary>
        public ulong Timestamp { get; set; }

        /// <summary>Block version</summary>
        public uint Version { get; set; }

        /// <summary>Block witness</summary>
        public WitnessModel? Witness { get; set; }
    }
}
