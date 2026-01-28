using System;
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
        public ExecuteResultScanner ExecuteResultScanner { get; }
        public ExecuteResultLogTracker ExecuteResultLogTracker { get; }
        
        private Task _scanTask;
        private bool _disposed;

        public GuiStarter()
        {
            ExecuteResultLogTracker = new ExecuteResultLogTracker();
            ExecuteResultScanner = new ExecuteResultScanner();
        }

        public override async Task Start(string[] args)
        {
            await base.Start(args);
            _scanTask = Task.Factory.StartNew(
                () => ExecuteResultScanner.Start(), 
                TaskCreationOptions.LongRunning);
            UnconfirmedTransactionCache.RegisterBlockPersistEvent(NeoSystem);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ExecuteResultScanner?.Dispose();
                    // Wait for scan task to complete
                    _scanTask?.Wait(TimeSpan.FromSeconds(5));
                }
                _disposed = true;
            }
        }
    }
}
