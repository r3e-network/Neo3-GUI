using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Neo.Ledger;
using Neo.Models;
using Neo.Models.Jobs;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;

namespace Neo.Common.Utility
{
    /// <summary>
    /// Thread-safe cache for unconfirmed transactions
    /// </summary>
    public class UnconfirmedTransactionCache
    {
        private static readonly ConcurrentDictionary<UInt256, TempTransaction> _unconfirmedTransactions = new();
        private static readonly object _actorLock = new();
        private static IActorRef _actor;

        /// <summary>
        /// Register for block persist events to remove confirmed transactions
        /// </summary>
        public static void RegisterBlockPersistEvent(NeoSystem neoSystem)
        {
            if (_actor == null)
            {
                lock (_actorLock)
                {
                    _actor ??= neoSystem.ActorSystem.ActorOf(
                        EventWrapper<Blockchain.PersistCompleted>.Props(Blockchain_PersistCompleted));
                }
            }
        }

        /// <summary>
        /// Add a transaction to the unconfirmed cache
        /// </summary>
        public static void AddTransaction(Transaction tx)
        {
            if (_unconfirmedTransactions.ContainsKey(tx.Hash))
            {
                return;
            }

            var transfers = new List<TransferNotifyItem>();
            using var exeResult = tx.Script.RunTestMode(null, tx);
            if (exeResult.Notifications.NotEmpty())
            {
                foreach (var notification in exeResult.Notifications)
                {
                    var transfer = notification.ConvertToTransfer();
                    if (transfer == null)
                    {
                        continue;
                    }
                    var asset = AssetCache.GetAssetInfo(transfer.Asset);
                    if (asset == null)
                    {
                        continue;
                    }
                    transfer.Symbol = asset.Symbol;
                    transfer.Decimals = asset.Decimals;
                    transfers.Add(transfer);
                }
            }
            SaveTransfer(tx, transfers);
        }

        private static void SaveTransfer(Transaction tx, List<TransferNotifyItem> transfers)
        {
            _unconfirmedTransactions[tx.Hash] = new TempTransaction()
            {
                Tx = tx,
                Transfers = transfers,
            };
        }

        /// <summary>
        /// Remove a confirmed transaction from the cache
        /// </summary>
        public static void RemoveUnconfirmedTransactions(UInt256 txId)
        {
            if (_unconfirmedTransactions.TryRemove(txId, out var confirmTransaction))
            {
                TransactionConfirmJob.AddConfirmedTransaction(confirmTransaction);
            }
        }

        /// <summary>
        /// Get an unconfirmed transaction by ID
        /// </summary>
        public static TempTransaction GetUnconfirmedTransaction(UInt256 txId)
        {
            return _unconfirmedTransactions.TryGetValue(txId, out var tx) ? tx : null;
        }

        /// <summary>
        /// Get paged list of unconfirmed transactions
        /// </summary>
        public static PageList<TempTransaction> GetUnconfirmedTransactions(
            IEnumerable<UInt160> addresses = null, 
            int pageIndex = 1, 
            int pageSize = 10)
        {
            IEnumerable<TempTransaction> query = _unconfirmedTransactions.Values;
            if (addresses.NotEmpty())
            {
                var addressSet = addresses.ToHashSet();
                query = query.Where(tx => tx.Transfers.Any(t => 
                    addressSet.Contains(t.From) || addressSet.Contains(t.To)));
            }
            var trans = query.Reverse().ToList();
            return new PageList<TempTransaction>
            {
                TotalCount = trans.Count,
                PageSize = pageSize,
                PageIndex = pageIndex,
                List = trans.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
            };
        }

        private static void Blockchain_PersistCompleted(Blockchain.PersistCompleted e)
        {
            if (e.Block.Transactions.NotEmpty())
            {
                foreach (var blockTransaction in e.Block.Transactions)
                {
                    RemoveUnconfirmedTransactions(blockTransaction.Hash);
                }
            }
        }

        public class TempTransaction
        {
            public Transaction Tx { get; set; }
            public List<TransferNotifyItem> Transfers { get; set; }
        }
    }
}
