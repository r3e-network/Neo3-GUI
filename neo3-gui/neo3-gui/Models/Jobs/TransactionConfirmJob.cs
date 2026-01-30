using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo.Common.Utility;

namespace Neo.Models.Jobs
{
    public class TransactionConfirmJob : Job
    {
        private static ConcurrentBag<UnconfirmedTransactionCache.TempTransaction> _confirmedTransactions = new();
        private static readonly object _swapLock = new();

        public TransactionConfirmJob(TimeSpan timeSpan)
        {
            IntervalTime = timeSpan;
        }

        public static void AddConfirmedTransaction(UnconfirmedTransactionCache.TempTransaction tx)
        {
            if (tx?.Tx != null)
            {
                _confirmedTransactions.Add(tx);
            }
        }

        public override async Task<WsMessage> Invoke()
        {
            // Atomically swap the bag to avoid losing transactions added during processing
            List<UnconfirmedTransactionCache.TempTransaction> snapshot;
            lock (_swapLock)
            {
                if (_confirmedTransactions.IsEmpty)
                {
                    return null;
                }
                snapshot = _confirmedTransactions.ToList();
                _confirmedTransactions = new ConcurrentBag<UnconfirmedTransactionCache.TempTransaction>();
            }

            var allConfirmTransactions = snapshot.Select(tx => tx.Tx.Hash).ToList();
            if (allConfirmTransactions.Count == 0)
            {
                return null;
            }

            var model = new ConfirmStateModel
            {
                Confirms = allConfirmTransactions,
            };

            var wallet = Program.Starter.CurrentWallet;
            if (wallet != null)
            {
                var accounts = wallet.GetAccounts().Select(a => a.ScriptHash).ToHashSet();
                model.MyConfirms = snapshot
                    .Where(tx => tx.Transfers.Any(t => accounts.Contains(t.From) || accounts.Contains(t.To)))
                    .Select(tx => tx.Tx.Hash)
                    .ToList();
            }

            return new WsMessage()
            {
                MsgType = WsMessageType.Push,
                Method = "getLastConfirmTransactions",
                Result = model,
            };
        }
    }
}
