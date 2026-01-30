using Neo.Models.Blocks;
using Neo.Network.P2P.Payloads;

namespace Neo.Extensions
{
    /// <summary>
    /// Block mapping extensions
    /// </summary>
    public static class BlockExtensions
    {
        public static BlockPreviewModel ToPreview(this Block block)
        {
            return new BlockPreviewModel
            {
                Hash = block.Hash,
                Index = block.Index,
                Timestamp = block.Timestamp.FromTimestampMS()
            };
        }
    }
}
