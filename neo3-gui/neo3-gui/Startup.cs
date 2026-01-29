using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo.Common;
using Neo.Models.Jobs;
using Neo.Services;

namespace Neo
{
    public class Startup
    {
        private const int WebSocketKeepAliveSeconds = 30;
        private const int WebSocketBufferSize = 4 * 1024;

        public IConfiguration Configuration { get; }
        public string ContentRootPath { get; set; }

        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration;

#if DEBUG
            var root = env.ContentRootPath;
            ContentRootPath = Path.Combine(root, "ClientApp");

            if (Directory.Exists(ContentRootPath))
            {
                CommandLineTool.Run("npm run dev", ContentRootPath);
            }
#endif
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // WebSocket services
            services.AddWebSocketInvoker();
            
            // Background services
            services.AddSingleton<NotificationService>();
            
            // Middleware
            services.AddSingleton<JsonRpcMiddleware>();
            
            // WebSocket options
            services.AddWebSockets(options =>
            {
                options.KeepAliveInterval = TimeSpan.FromSeconds(WebSocketKeepAliveSeconds);
                options.ReceiveBufferSize = WebSocketBufferSize;
            });

            // Memory cache for hot data
            services.AddMemoryCache();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseWebSockets();
            app.UseMiddleware<JsonRpcMiddleware>();
            app.UseMiddleware<WebSocketHubMiddleware>();

            ConfigureNotificationJobs(app);
        }

        private static void ConfigureNotificationJobs(IApplicationBuilder app)
        {
            var notify = app.UseNotificationService();
            notify.Register(new SyncHeightJob(TimeSpan.FromSeconds(5)));
            notify.Register(new SyncWalletJob(TimeSpan.FromSeconds(11)));
            notify.Register(new TransactionConfirmJob(TimeSpan.FromSeconds(7)));
        }
    }
}
