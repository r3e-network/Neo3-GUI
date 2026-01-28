using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Neo;
using Neo.Common.Consoles;
using Neo.Ledger;

namespace neo3_gui.tests
{
    public static class TestBlockchain
    {
        public static readonly NeoSystem TheNeoSystem;

        static TestBlockchain()
        {
            var configPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "neo3-gui", "config.testnet.json"));
            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException($"Protocol settings not found at {configPath}");
            }
            var settings = ProtocolSettings.Load(configPath);
            TheNeoSystem = new NeoSystem(settings);
            var programType = typeof(Helpers).Assembly.GetType("Neo.Program");
            var starterField = programType?.GetField("Starter", BindingFlags.Public | BindingFlags.Static);
            var starter = starterField?.GetValue(null) as GuiStarter ?? new GuiStarter();
            starterField?.SetValue(null, starter);
            var field = typeof(MainService).GetField("neoSystem", BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(starter, TheNeoSystem);
        }
    }
}
