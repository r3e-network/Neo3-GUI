using System;

namespace Neo.Models
{
    public class HeightStateModel
    {
        public uint ScanHeight { get; set; }
        public uint SyncHeight { get; set; }
        public uint HeaderHeight { get; set; }

        public int ConnectedCount { get; set; }

        public DateTime Time { get; set; } = DateTime.Now;
    }
}
