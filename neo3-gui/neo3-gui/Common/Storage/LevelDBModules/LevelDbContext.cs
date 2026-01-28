using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using Neo.Common.Consoles;
using Neo.Extensions;
using Neo.Models;
using Neo.VM.Types;

namespace Neo.Common.Storage.LevelDBModules
{
    /// <summary>
    /// LevelDB context for blockchain data storage
    /// </summary>
    public class LevelDbContext : IDisposable
    {
        private static readonly ConcurrentDictionary<string, DB> _cache = new();

        private readonly DB _db;
        private WriteBatch _writeBatch = new();
        private bool _disposed;

        private static readonly byte[] ExecuteLogPrefix = { 0xff };
        private static readonly byte[] MaxSyncIndexPrefix = { 0xfd };
        private static readonly byte[] AssetPrefix = { 0xfc };
        private static readonly byte[] BalancePrefix = { 0xfb };
        private static readonly byte[] TransferPrefix = { 0xfa };
        private static readonly byte[] ContractEventPrefix = { 0xf9 };
        private static readonly byte[] BalanceChangingPrefix = { 0xf8 };

        static LevelDbContext()
        {
            if (!Directory.Exists("Data_Track"))
            {
                Directory.CreateDirectory("Data_Track");
            }
        }

        public LevelDbContext() 
            : this(Path.Combine("Data_Track", $"TransactionLog_LevelDB_{CliSettings.Default.Protocol.Network}"))
        {
        }

        public LevelDbContext(string dbPath)
        {
            _db = _cache.GetOrAdd(dbPath, path => 
                DB.Open(path, new Options { CreateIfMissing = true }));
        }

        private byte[] ContractEventKey(uint height) => ContractEventPrefix.Append(BitConverter.GetBytes(height));

        public Dictionary<UInt256, List<ContractEventInfo>> GetContractEvent(uint height)
        {
            var key = ContractEventKey(height);
            var value = _db.Get(key);
            if (value.NotEmpty())
            {
                var storageValue = value.DeserializeJson<List<ContractEventStorageItem>>();
                return storageValue?.ToDictionary(s => s.TxId, s => s.Events);
            }
            return null;
        }

        /// <summary>
        /// Save current block contract change (create, destroy, migrate) info
        /// </summary>
        public void SaveContractEvent(uint height, IDictionary<UInt256, List<ContractEventInfo>> value)
        {
            if (value.NotEmpty())
            {
                var key = ContractEventKey(height);
                var storageValue = value.Select(kv => new ContractEventStorageItem
                {
                    TxId = kv.Key,
                    Events = kv.Value,
                });
                _writeBatch.Put(key, storageValue.SerializeJsonBytes());
            }
        }

        private byte[] ExecuteLogKey(UInt256 txId) => ExecuteLogPrefix.Append(txId.ToArray());

        /// <summary>
        /// Save execute result log after call Commit method
        /// </summary>
        public void SaveTxExecuteLog(ExecuteResultInfo log)
        {
            if (log?.TxId != null)
            {
                try
                {
                    _writeBatch.Put(ExecuteLogKey(log.TxId), log.SerializeJsonBytes());
                }
                catch (Exception e)
                {
                    log.ResultStack = new StackItem[] { e.ToString() };
                    _writeBatch.Put(ExecuteLogKey(log.TxId), log.SerializeJsonBytes());
                }
            }
        }

        public ExecuteResultInfo GetExecuteLog(UInt256 txId)
        {
            var value = _db.Get(ReadOptions.Default, ExecuteLogKey(txId));
            if (value.NotEmpty())
            {
                return Encoding.UTF8.GetString(value).DeserializeJson<ExecuteResultInfo>();
            }
            return null;
        }

        private byte[] AssetKey(UInt160 assetId) => AssetPrefix.Append(assetId.ToArray());

        public void SaveAssetInfo(AssetInfo assetInfo)
        {
            var key = AssetKey(assetInfo.Asset);
            var value = _db.Get(key);
            if (value == null)
            {
                _writeBatch.Put(key, assetInfo.SerializeJsonBytes());
            }
        }

        public AssetInfo GetAssetInfo(UInt160 assetId)
        {
            var key = AssetKey(assetId);
            var value = _db.Get(ReadOptions.Default, key);
            return value?.DeserializeJson<AssetInfo>();
        }

        private byte[] BalanceAccountsKey(uint height) => BalanceChangingPrefix.Append(BitConverter.GetBytes(height));

        /// <summary>
        /// Save Balance-Changed Accounts in Specified Block
        /// </summary>
        public void UpdateBalancingAccounts(uint height, HashSet<AccountAsset> accounts)
        {
            var key = BalanceAccountsKey(height);
            _writeBatch.Put(key, accounts.SerializeJsonBytes());
        }

        /// <summary>
        /// Get Balance-Changed Accounts in Specified Block
        /// </summary>
        public HashSet<AccountAsset> GetBalancingAccounts(uint height)
        {
            var key = BalanceAccountsKey(height);
            return _db.Get(key).DeserializeJson<HashSet<AccountAsset>>();
        }

        private byte[] BalanceKey(UInt160 account, UInt160 asset) => BalancePrefix.Append(account.ToArray(), asset.ToArray());

        public void UpdateBalance(UInt160 account, UInt160 asset, BigInteger balance, uint height)
        {
            var key = BalanceKey(account, asset);
            var balanceRecord = new BalanceStorageItem { Balance = balance, Height = height };
            _writeBatch.Put(key, balanceRecord.SerializeJsonBytes());
        }

        public BalanceStorageItem GetBalance(UInt160 account, UInt160 asset)
        {
            var key = BalanceKey(account, asset);
            return GetBalance(key);
        }

        private BalanceStorageItem GetBalance(byte[] balanceKey)
        {
            var value = _db.Get(balanceKey);
            return value.NotEmpty() ? value.DeserializeJson<BalanceStorageItem>() : null;
        }

        private byte[] TransferKey(uint blockHeight) => TransferPrefix.Append(BitConverter.GetBytes(blockHeight));

        /// <summary>
        /// Save transfers after call Commit method
        /// </summary>
        public void SaveTransfers(uint blockHeight, List<TransferStorageItem> transfers)
        {
            var key = TransferKey(blockHeight);
            _writeBatch.Put(key, transfers.SerializeJsonBytes());
        }

        public List<TransferStorageItem> GetTransfers(uint blockHeight)
        {
            var key = TransferKey(blockHeight);
            var value = _db.Get(key);
            return value.NotEmpty() ? value.DeserializeJson<List<TransferStorageItem>>() : null;
        }

        private byte[] MaxSyncIndexKey(byte[] db) => MaxSyncIndexPrefix.Append(db);

        /// <summary>
        /// Check if height is synced
        /// </summary>
        public bool HasSyncIndex(byte[] dbPrefix, uint height)
        {
            var max = GetMaxSyncIndex(dbPrefix);
            return max >= height;
        }

        /// <summary>
        /// Save synced max height after call Commit method
        /// </summary>
        public void SetMaxSyncIndex(byte[] dbPrefix, uint height)
        {
            var max = GetMaxSyncIndex(dbPrefix);
            if (max == null || max < height)
            {
                SetMaxSyncIndexForce(dbPrefix, height);
            }
        }

        /// <summary>
        /// Force save synced max height after call Commit method
        /// </summary>
        public void SetMaxSyncIndexForce(byte[] dbPrefix, uint height)
        {
            _writeBatch.Put(MaxSyncIndexKey(dbPrefix), BitConverter.GetBytes(height));
        }

        public uint? GetMaxSyncIndex(byte[] dbPrefix)
        {
            var max = _db.Get(ReadOptions.Default, MaxSyncIndexKey(dbPrefix));
            return max == null ? null : BitConverter.ToUInt32(max);
        }

        public void Commit()
        {
            _db.Write(WriteOptions.Default, _writeBatch);
            ResetWriteBatch();
        }

        private void ResetWriteBatch()
        {
            _writeBatch?.Dispose();
            _writeBatch = new WriteBatch();
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
                    _writeBatch?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
