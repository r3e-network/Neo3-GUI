using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Neo.Common;

namespace Neo
{
    class Program
    {
        public static GuiStarter Starter = new GuiStarter();

        static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            await Starter.Start(args);
            var host = CreateHostBuilder(args).Build();
            await host.StartAsync();
            Starter.RunConsole();
            Starter.Stop();
            await host.StopAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var url = "http://127.0.0.1:8081";
            var envPort = Environment.GetEnvironmentVariable("NEO_GUI_PORT");
            if (int.TryParse(envPort, out var port))
            {
                url = $"http://127.0.0.1:{port}";
            }
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls(url)
                        .UseStartup<Startup>();
                });
        }


        public static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            CommandLineTool.Close();
        }
    }
}
