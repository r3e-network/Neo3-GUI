using Neo.Services.Abstractions;
using Neo.SmartContract;
using Neo.VM;

namespace Neo.Services.Core
{
    /// <summary>
    /// Script execution service implementation
    /// </summary>
    public class ScriptExecutor : IScriptExecutor
    {
        private readonly IBlockchainService _blockchain;

        public ScriptExecutor(IBlockchainService blockchain)
        {
            _blockchain = blockchain;
        }

        public ApplicationEngine Execute(byte[] script)
        {
            return script.RunTestMode(_blockchain.GetSnapshot());
        }

        public bool IsSuccess(ApplicationEngine engine)
        {
            return !engine.State.HasFlag(VMState.FAULT);
        }
    }
}
