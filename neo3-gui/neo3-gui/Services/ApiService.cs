using System;
using System.Threading;
using System.Threading.Tasks;
using Neo.Common;
using Neo.Models;
using Neo.Models.Contracts;
using Neo.Network.P2P.Payloads;
using Neo.Common.Consoles;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.Wallets;

namespace Neo.Services
{
    public interface IApiService { }
    public abstract class ApiService : IApiService
    {
        private const string InsufficientGasMessage = "Insufficient GAS";
        private const byte TransactionVersion = 0;

        protected Wallet CurrentWallet => Program.Starter.CurrentWallet;

        private readonly AsyncLocal<IWebSocketConnection> _asyncClient = new AsyncLocal<IWebSocketConnection>();

        public IWebSocketConnection Client
        {
            get => _asyncClient.Value;
            set => _asyncClient.Value = value;
        }


        protected WsError Error(ErrorCode code)
        {
            return code.ToError();
        }
        protected WsError Error(ErrorCode code, string message)
        {
            return new WsError() { Code = (int)code, Message = message };
        }

        protected WsError Error(int code, string message)
        {
            return new WsError() { Code = code, Message = message };
        }

        protected async Task<object> SignAndBroadcastTxWithSender(byte[] script, UInt160 sender, params UInt160[] signers)
        {
            Transaction tx;
            try
            {
                tx = CurrentWallet.InitTransaction(script, sender, signers);
            }
            catch (InvalidOperationException ex)
            {
                return Error(ErrorCode.EngineFault, ex.Message);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(InsufficientGasMessage))
                {
                    return Error(ErrorCode.GasNotEnough);
                }
                throw;
            }
            var (signSuccess, context) = CurrentWallet.TrySignTx(tx);
            if (!signSuccess)
            {
                return Error(ErrorCode.SignFail, context.SafeSerialize());
            }
            var result = new TxResultModel();
            await tx.Broadcast();
            result.TxId = tx.Hash;
            return result;
        }

        protected async Task<object> SignAndBroadcastTx(byte[] script, params UInt160[] signers)
        {
            return await SignAndBroadcastTxWithSender(script, null, signers);
        }

        /// <summary>
        /// Sign and broadcast transaction with fixed system fee (for registerCandidate which requires 1000 GAS)
        /// </summary>
        protected async Task<object> SignAndBroadcastTxWithFixedFee(byte[] script, UInt160 account, long fixedSystemFee)
        {
            var snapshot = Helpers.GetDefaultSnapshot();
            var tx = new Transaction
            {
                Version = TransactionVersion,
                Nonce = (uint)new Random().Next(),
                Script = script,
                ValidUntilBlock = NativeContract.Ledger.CurrentIndex(snapshot) + CliSettings.Default.Protocol.MaxValidUntilBlockIncrement,
                Signers = new[] { new Signer { Account = account, Scopes = WitnessScope.CalledByEntry } },
                Attributes = Array.Empty<TransactionAttribute>(),
                SystemFee = fixedSystemFee,
                NetworkFee = 0
            };

            // Calculate network fee
            var data = new ContractParametersContext(snapshot, tx, CliSettings.Default.Protocol.Network);
            CurrentWallet.Sign(data);
            tx.Witnesses = data.GetWitnesses();
            tx.NetworkFee = CurrentWallet.CalculateNetworkFee(snapshot, tx, CliSettings.Default.Protocol.MaxVerificationGas);

            // Re-sign with correct fees
            var context = new ContractParametersContext(snapshot, tx, CliSettings.Default.Protocol.Network);
            CurrentWallet.Sign(context);
            if (!context.Completed)
            {
                return Error(ErrorCode.SignFail, context.SafeSerialize());
            }
            tx.Witnesses = context.GetWitnesses();

            var result = new TxResultModel();
            await tx.Broadcast();
            result.TxId = tx.Hash;
            return result;
        }
    }
}