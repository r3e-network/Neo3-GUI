namespace Neo.Configuration
{
    /// <summary>
    /// Application configuration with validation
    /// </summary>
    public class NeoGuiOptions
    {
        public const string SectionName = "NeoGui";

        public int WebSocketKeepAliveSeconds { get; set; } = 30;
        public int WebSocketBufferSize { get; set; } = 4096;
        public int DefaultPort { get; set; } = 8081;
        public string ListenAddress { get; set; } = "127.0.0.1";

        public void Validate()
        {
            if (WebSocketKeepAliveSeconds < 1)
                throw new ArgumentException(
                    "WebSocketKeepAliveSeconds must be >= 1");

            if (WebSocketBufferSize < 1024)
                throw new ArgumentException(
                    "WebSocketBufferSize must be >= 1024");

            if (DefaultPort < 1 || DefaultPort > 65535)
                throw new ArgumentException(
                    "DefaultPort must be between 1 and 65535");
        }
    }
}
