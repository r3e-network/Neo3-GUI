using System;
using System.Threading;
using System.Threading.Tasks;
using Neo.Common.Analyzers;
using Neo.Common.Consoles;
using Neo.Common.Scanners;
using Neo.Common.Utility;

namespace Neo
{
    /// <summary>
    /// GUI application starter that extends MainService
    /// </summary>
    public class GuiStarter : MainService, IDisposable
    {
        private const int ScanTaskWaitTimeoutSeconds = 5;

        public ExecuteResultScanner ExecuteResultScanner { get; }
        public ExecuteResultLogTracker ExecuteResultLogTracker { get; }
        
        private Task _scanTask;
        private CancellationTokenSource _scanCts;
        private volatile bool _disposed;
        private volatile bool _started;

        public GuiStarter()
        {
            ExecuteResultLogTracker = new ExecuteResultLogTracker();
            ExecuteResultScanner = new ExecuteResultScanner();
            _scanCts = new CancellationTokenSource();
        }

        public override async Task Start(string[] args)
        {
            if (_started) return;
            _started = true;

            await base.Start(args);
            
            _scanTask = Task.Factory.StartNew(
                () => ExecuteResultScanner.Start(), 
                _scanCts.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
            
            UnconfirmedTransactionCache.RegisterBlockPersistEvent(NeoSystem);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;

            if (disposing)
            {
                // Cancel scan task
                _scanCts?.Cancel();
                
                // Dispose scanner
                ExecuteResultScanner?.Dispose();
                
                // Wait for scan task
                try
                {
                    _scanTask?.Wait(TimeSpan.FromSeconds(ScanTaskWaitTimeoutSeconds));
                }
                catch (AggregateException)
                {
                    // Task may have been cancelled
                }
                
                _scanCts?.Dispose();
            }
        }
    }
}
