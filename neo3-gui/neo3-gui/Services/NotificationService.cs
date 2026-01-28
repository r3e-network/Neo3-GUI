using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Neo.Common;
using Neo.Models.Jobs;

namespace Neo.Services
{
    /// <summary>
    /// Background notification service for pushing messages to WebSocket clients
    /// </summary>
    public class NotificationService
    {
        private readonly WebSocketHub _hub;
        private readonly ConcurrentBag<Job> _jobs = new();
        private volatile bool _running = true;

        public NotificationService(WebSocketHub hub)
        {
            _hub = hub ?? throw new ArgumentNullException(nameof(hub));
        }

        public async Task Start()
        {
            while (_running)
            {
                foreach (var job in _jobs)
                {
                    try
                    {
                        var now = DateTime.Now;
                        if (job.NextTriggerTime <= now)
                        {
                            var msg = await job.Invoke();
                            if (msg != null)
                            {
                                _hub.PushAll(msg);
                            }
                            job.LastTriggerTime = now;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Job Error[{job}]: {e.Message}");
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        public void Register(Job job)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            _jobs.Add(job);
        }

        public void Stop()
        {
            _running = false;
        }
    }

    public static class NotificationServiceExtension
    {
        public static NotificationService UseNotificationService(this IApplicationBuilder app)
        {
            var service = app.ApplicationServices.GetRequiredService<NotificationService>();
            _ = Task.Run(service.Start);
            return service;
        }
    }
}
