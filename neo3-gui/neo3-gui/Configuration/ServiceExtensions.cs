using Microsoft.Extensions.DependencyInjection;
using Neo.Services.Abstractions;
using Neo.Services.Core;
using Neo.Services.Strategies;

namespace Neo.Configuration
{
    /// <summary>
    /// Service registration extensions
    /// </summary>
    public static class ServiceExtensions
    {
        public static IServiceCollection AddNeoServices(
            this IServiceCollection services)
        {
            // Core services
            services.AddSingleton<IBlockchainService, BlockchainService>();
            services.AddSingleton<IBalanceService, BalanceService>();
            services.AddSingleton<IJsonService, JsonService>();
            services.AddSingleton<ITransactionBroadcastService, TransactionBroadcastService>();
            services.AddSingleton<IHealthCheckService, HealthCheckService>();
            services.AddSingleton<INeoLogger, NeoLogger>();
            services.AddSingleton<IAssetService, AssetService>();
            services.AddSingleton<IWalletService, WalletService>();
            services.AddSingleton<IMetricsService, MetricsService>();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddSingleton<ITransactionFactory, TransactionFactory>();
            services.AddSingleton<IEventPublisher, EventPublisher>();
            
            // Strategies
            services.AddSingleton<IAssetConversionStrategy, AssetConversionStrategy>();
            services.AddSingleton<IScriptExecutor, ScriptExecutor>();
            services.AddSingleton<IAddressValidator, AddressValidator>();
            services.AddSingleton<IContractChecker, ContractChecker>();
            services.AddSingleton<ICandidateService, CandidateService>();
            
            return services;
        }
    }
}
