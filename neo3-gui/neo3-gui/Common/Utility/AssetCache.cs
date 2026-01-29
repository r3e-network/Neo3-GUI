using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Neo.Common.Storage.SQLiteModules;
using Neo.Common.Storage.LevelDBModules;
using Neo.Extensions;
using Neo.Models;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;

namespace Neo.Common.Utility
{
    /// <summary>
    /// Cache for NEP-17/NEP-11 asset information
    /// </summary>
    public class AssetCache
    {
        private static readonly ConcurrentDictionary<UInt160, AssetInfo> _assets = new();

        /// <summary>
        /// Read NEP-17 asset info from cache first
        /// </summary>
        /// <param name="assetId">Asset contract hash</param>
        /// <param name="readFromDb">Whether to read from database if not in cache</param>
        /// <returns>Asset info or null if not found</returns>
        public static AssetInfo GetAssetInfo(UInt160 assetId, bool readFromDb = true)
        {
            if (_assets.TryGetValue(assetId, out var cached))
            {
                return cached;
            }
            var snapshot = Helpers.GetDefaultSnapshot();
            return GetAssetInfo(assetId, snapshot, readFromDb);
        }

        /// <summary>
        /// Read NEP-17 asset info from cache first
        /// </summary>
        /// <param name="assetId">Asset contract hash</param>
        /// <param name="snapshot">Data cache snapshot</param>
        /// <param name="readFromDb">Whether to read from database if not in cache</param>
        /// <returns>Asset info or null if not found</returns>
        public static AssetInfo GetAssetInfo(UInt160 assetId, DataCache snapshot, bool readFromDb = true)
        {
            if (_assets.TryGetValue(assetId, out var cached))
            {
                return cached;
            }
            return GetAssetInfoFromChain(assetId, snapshot) 
                   ?? (readFromDb ? GetAssetInfoFromLevelDb(assetId) : null);
        }

        /// <summary>
        /// Read NEP-17 asset info from chain and cache it
        /// https://github.com/neo-project/proposals/blob/master/nep-17.mediawiki
        /// </summary>
        /// <param name="assetId">Asset contract hash</param>
        /// <param name="snapshot">Data cache snapshot</param>
        /// <returns>Asset info or null if not a valid asset</returns>
        public static AssetInfo GetAssetInfoFromChain(UInt160 assetId, DataCache snapshot)
        {
            var contract = snapshot.GetContract(assetId);
            if (contract == null)
            {
                return null;
            }
            var assetType = contract.CheckNepAsset();
            if (assetType == AssetType.None)
            {
                return null;
            }
            try
            {
                using var sb = new ScriptBuilder();
                sb.EmitDynamicCall(assetId, "decimals");
                sb.EmitDynamicCall(assetId, "symbol");
                using var engine = sb.ToArray().RunTestMode(snapshot);
                if (engine.State.HasFlag(VMState.FAULT))
                {
                    Console.WriteLine($"Contract [{assetId}] is not Asset at height:{snapshot.GetHeight()}");
                    return null;
                }
                string name = contract.Manifest.Name;
                string symbol = engine.ResultStack.Pop().GetString();
                byte decimals = (byte)engine.ResultStack.Pop().GetInteger();
                symbol = symbol == "neo" || symbol == "gas" ? symbol.ToUpper() : symbol;
                var assetInfo = new AssetInfo()
                {
                    Asset = assetId,
                    Decimals = decimals,
                    Symbol = symbol,
                    Name = name,
                    Type = assetType,
                };

                _assets[assetId] = assetInfo;
                return assetInfo;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Invalid Asset[{assetId}]:{e}");
                return null;
            }
        }

        /// <summary>
        /// Read asset info from backup LevelDB
        /// </summary>
        /// <param name="assetId">Asset contract hash</param>
        /// <returns>Asset info or null if not found</returns>
        public static AssetInfo GetAssetInfoFromLevelDb(UInt160 assetId)
        {
            using var db = new LevelDbContext();
            var oldAsset = db.GetAssetInfo(assetId);
            if (oldAsset != null)
            {
                var asset = new AssetInfo()
                {
                    Asset = assetId,
                    Decimals = oldAsset.Decimals,
                    Name = oldAsset.Name,
                    Symbol = oldAsset.Symbol,
                    Type = oldAsset.Type,
                };
                _assets[assetId] = asset;
                return asset;
            }
            return null;
        }

        /// <summary>
        /// Get total supply of an asset
        /// </summary>
        /// <param name="asset">Asset contract hash</param>
        /// <returns>Total supply or null if failed</returns>
        public static BigDecimal? GetTotalSupply(UInt160 asset)
        {
            var snapshot = Helpers.GetDefaultSnapshot();
            using var sb = new ScriptBuilder();
            sb.EmitDynamicCall(asset, "totalSupply");
            using var engine = sb.ToArray().RunTestMode(snapshot);
            
            if (engine.State.HasFlag(VMState.FAULT))
            {
                return null;
            }
            
            var total = engine.ResultStack.FirstOrDefault()?.ToBigInteger();
            var assetInfo = GetAssetInfo(asset);
            return total.HasValue && assetInfo != null 
                ? new BigDecimal(total.Value, assetInfo.Decimals) 
                : null;
        }

        /// <summary>
        /// Get total supply for multiple assets
        /// </summary>
        /// <param name="assets">Asset contract hashes</param>
        /// <returns>List of total supplies (null for failed queries)</returns>
        public static List<BigDecimal?> GetTotalSupply(IEnumerable<UInt160> assets)
        {
            if (assets.IsEmpty())
            {
                return new List<BigDecimal?>();
            }

            var assetList = assets.ToList();
            var results = new List<BigDecimal?>(assetList.Count);
            var snapshot = Helpers.GetDefaultSnapshot();

            foreach (var asset in assetList)
            {
                var assetInfo = GetAssetInfo(asset);
                if (assetInfo == null)
                {
                    results.Add(null);
                    continue;
                }

                using var sb = new ScriptBuilder();
                sb.EmitDynamicCall(asset, "totalSupply");
                using var engine = sb.ToArray().RunTestMode(snapshot);
                
                if (engine.State == VMState.FAULT)
                {
                    Console.WriteLine($"{asset} has invalid totalSupply");
                    results.Add(null);
                }
                else
                {
                    var totalSupply = engine.ResultStack.Pop().ToBigInteger();
                    results.Add(totalSupply.HasValue 
                        ? new BigDecimal(totalSupply.Value, assetInfo.Decimals) 
                        : null);
                }
            }
            return results;
        }
    }
}
