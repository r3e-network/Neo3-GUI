using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo.Common;
using Neo.Models.Jobs;

namespace Neo.Services
{
    /// <summary>
    /// Background notification service for pushing messages to WebSocket clients
    /// </summary>
    public class NotificationService : IDisposable
    {
        private const int JobLoopDelayMs = 1000;
        private const int MaxConsecutiveErrors = 5;
        private const int ErrorBackoffMs = 5000;

        private readonly WebSocketHub _hub;
        private readonly ConcurrentBag<Job> _jobs = new();
        private readonly CancellationTokenSource _cts = new();
        private readonly ILogger<NotificationService> _logger;
        
        private volatile bool _running;
        private bool _disposed;

        public NotificationService(
            WebSocketHub hub,
            ILogger<NotificationService> logger = null)
        {
            _hub = hub ?? throw new ArgumentNullException(nameof(hub));
            _logger = logger;
        }

        /// <summary>
        /// Start the notification service loop
        /// </summary>
        public async Task Start()
        {
            if (_running) return;
            _running = true;

            _logger?.LogInformation("NotificationService started");

            try
            {
                await RunJobLoop(_cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Expected on shutdown
            }
            finally
            {
                _running = false;
                _logger?.LogInformation("NotificationService stopped");
            }
        }

        private async Task RunJobLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                foreach (var job in _jobs)
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    await ExecuteJobSafe(job);
                }

                await Task.Delay(JobLoopDelayMs, cancellationToken);
            }
        }

        private async Task ExecuteJobSafe(Job job)
        {
            try
            {
                var now = DateTime.Now;
                if (job.NextTriggerTime > now) return;

                var msg = await job.Invoke();
                if (msg != null)
                {
                    _hub.PushAll(msg);
                }
                
                job.LastTriggerTime = now;
                job.ConsecutiveErrors = 0;
            }
            catch (Exception ex)
            {
                job.ConsecutiveErrors++;
                _logger?.LogWarning(ex, 
                    "Job {JobType} error (attempt {Count})", 
                    job.GetType().Name, 
                    job.ConsecutiveErrors);

                // Back off if too many errors
                if (job.ConsecutiveErrors >= MaxConsecutiveErrors)
                {
                    job.LastTriggerTime = DateTime.Now.AddMilliseconds(ErrorBackoffMs);
                    _logger?.LogWarning(
                        "Job {JobType} backing off for {Ms}ms", 
                        job.GetType().Name, 
                        ErrorBackoffMs);
                }
            }
        }

        /// <summary>
        /// Register a job for periodic execution
        /// </summary>
        public void Register(Job job)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            _jobs.Add(job);
            _logger?.LogInformation("Registered job: {JobType}", job.GetType().Name);
        }

        /// <summary>
        /// Stop the notification service
        /// </summary>
        public void Stop()
        {
            if (!_running) return;
            _cts.Cancel();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                Stop();
                _cts.Dispose();
            }

            _disposed = true;
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
