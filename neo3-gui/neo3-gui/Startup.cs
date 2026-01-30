using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo.Common;
using Neo.Configuration;
using Neo.Models.Jobs;
using Neo.Services;
using Neo.Services.Abstractions;
using Neo.Services.Core;

namespace Neo
{
    public class Startup
    {
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
            // Configuration
            var options = new NeoGuiOptions();
            Configuration.GetSection(NeoGuiOptions.SectionName).Bind(options);
            options.Validate();
            services.AddSingleton(options);

            // Core services (using extension method)
            services.AddNeoServices();

            // WebSocket services
            services.AddWebSocketInvoker();
            
            // Background services
            services.AddSingleton<NotificationService>();
            
            // Middleware
            services.AddSingleton<JsonRpcMiddleware>();
            
            // WebSocket options
            services.AddWebSockets(opt =>
            {
                opt.KeepAliveInterval = TimeSpan.FromSeconds(options.WebSocketKeepAliveSeconds);
                opt.ReceiveBufferSize = options.WebSocketBufferSize;
            });

            // Memory cache and logging
            services.AddMemoryCache();
            services.AddLogging();
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
