using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Neo.Network.P2P;

namespace Neo.Common.Consoles
{
    public class CliSettings
    {
        private const string ConfigFileName = "config";
        private const string ApplicationConfigSection = "ApplicationConfiguration";

        public ProtocolSettings Protocol { get; private set; }
        public StorageSettings Storage { get; }
        public P2PSettings P2P { get; }
        public UnlockWalletSettings UnlockWallet { get; }
        public string PluginURL { get; }

        static CliSettings _default;

        static bool UpdateDefault(IConfiguration configuration)
        {
            var settings = new CliSettings(configuration.GetSection(ApplicationConfigSection));
            settings.Protocol = ProtocolSettings.Load(ConfigFileName.GetEnvConfigPath());
            return null == Interlocked.CompareExchange(ref _default, settings, null);
        }



        public static CliSettings Default
        {
            get
            {
                if (_default == null)
                {
                    UpdateDefault(ConfigFileName.LoadConfig());
                }
                return _default;
            }
        }

        public CliSettings(IConfigurationSection section)
        {
            this.Storage = new StorageSettings(section.GetSection("Storage"));
            this.P2P = new P2PSettings(section.GetSection("P2P"));
            this.UnlockWallet = new UnlockWalletSettings(section.GetSection("UnlockWallet"));
            this.PluginURL = section.GetSection("PluginURL").Value;
        }
    }

    public class LoggerSettings
    {
        private const string DefaultLogPath = "Logs_{0}";

        public string Path { get; }
        public bool ConsoleOutput { get; }
        public bool Active { get; }

        public LoggerSettings(IConfigurationSection section)
        {
            this.Path = section.GetValue("Path", DefaultLogPath);
            this.ConsoleOutput = section.GetValue("ConsoleOutput", false);
            this.Active = section.GetValue("Active", false);
        }
    }

    public class StorageSettings
    {
        private const string DefaultEngine = "LevelDBStore";
        private const string DefaultStoragePath = "Data_LevelDB_{0}";

        public string Engine { get; }
        public string Path { get; }

        public StorageSettings(IConfigurationSection section)
        {
            this.Engine = section.GetValue("Engine", DefaultEngine);
            this.Path = section.GetValue("Path", DefaultStoragePath);
        }
    }

    public class P2PSettings
    {
        private const string DefaultPort = "10333";
        private const string DefaultWsPort = "10334";
        private const int DefaultMaxConnectionsPerAddress = 3;

        public ushort Port { get; }
        public ushort WsPort { get; }
        public int MinDesiredConnections { get; }
        public int MaxConnections { get; }
        public int MaxConnectionsPerAddress { get; }
        public int MaxKnownHashes { get; }

        public P2PSettings(IConfigurationSection section)
        {
            this.Port = ushort.Parse(section.GetValue("Port", DefaultPort));
            this.WsPort = ushort.Parse(section.GetValue("WsPort", DefaultWsPort));
            this.MinDesiredConnections = section.GetValue("MinDesiredConnections", ChannelsConfig.DefaultMinDesiredConnections);
            this.MaxConnections = section.GetValue("MaxConnections", ChannelsConfig.DefaultMaxConnections);
            this.MaxKnownHashes = section.GetValue("MaxKnownHashes", ChannelsConfig.DefaultMaxKnownHashes);
            this.MaxConnectionsPerAddress = section.GetValue("MaxConnectionsPerAddress", DefaultMaxConnectionsPerAddress);
        }
    }

    public class UnlockWalletSettings
    {
        public string Path { get; }
        public string Password { get; }
        public bool IsActive { get; }

        public UnlockWalletSettings(IConfigurationSection section)
        {
            if (section.Exists())
            {
                this.Path = section.GetValue("Path", "");
                this.Password = section.GetValue("Password", "");
                this.IsActive = bool.Parse(section.GetValue("IsActive", "false"));
            }
        }
    }

}
