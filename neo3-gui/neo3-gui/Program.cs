using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Neo.Common;

namespace Neo
{
    class Program
    {
        private const string DefaultUrl = "http://127.0.0.1:8081";
        private const string PortEnvVar = "NEO_GUI_PORT";
        private const int ShutdownTimeoutSeconds = 10;

        public static GuiStarter Starter = new GuiStarter();

        static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                await Starter.Start(args);
                
                using var host = CreateHostBuilder(args).Build();
                await host.StartAsync();
                
                Console.WriteLine("Neo3-GUI started successfully");
                
                Starter.RunConsole();
                
                // Graceful shutdown
                await ShutdownAsync(host);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
                Environment.ExitCode = 1;
            }
        }

        private static async Task ShutdownAsync(IHost host)
        {
            Console.WriteLine("Shutting down...");
            
            Starter.Stop();
            
            using var cts = new System.Threading.CancellationTokenSource(
                TimeSpan.FromSeconds(ShutdownTimeoutSeconds));
            
            try
            {
                await host.StopAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Shutdown timed out, forcing exit");
            }
            
            Starter.Dispose();
            CommandLineTool.Close();
            
            Console.WriteLine("Shutdown complete");
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var url = GetListenUrl();
            
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls(url)
                        .UseStartup<Startup>();
                });
        }

        private static string GetListenUrl()
        {
            var envPort = Environment.GetEnvironmentVariable(PortEnvVar);
            if (int.TryParse(envPort, out var port) && port > 0 && port < 65536)
            {
                return $"http://127.0.0.1:{port}";
            }
            return DefaultUrl;
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            CommandLineTool.Close();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine($"Unhandled exception: {e.ExceptionObject}");
        }
    }
}
