using System.Numerics;
using Neo.Cryptography.ECC;
using Neo.Services.Abstractions;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Services.Core
{
    /// <summary>
    /// Candidate service implementation
    /// </summary>
    public class CandidateService : ICandidateService
    {
        private readonly IBlockchainService _blockchain;

        public CandidateService(IBlockchainService blockchain)
        {
            _blockchain = blockchain;
        }

        public (ECPoint PublicKey, BigInteger Votes)[] GetCandidates()
        {
            using var sb = new ScriptBuilder();
            sb.EmitDynamicCall(NativeContract.NEO.Hash, "getCandidates");
            
            var engine = sb.ToArray().RunTestMode(_blockchain.GetSnapshot());
            var array = engine.ResultStack.Pop() as Array;
            
            if (array == null || array.Count == 0)
                return System.Array.Empty<(ECPoint, BigInteger)>();

            return array.Cast<Struct>()
                .Select(item => (
                    ECPoint.FromBytes(item[0].GetSpan().ToArray(), ECCurve.Secp256r1),
                    item[1].GetInteger()))
                .ToArray();
        }
    }
}
