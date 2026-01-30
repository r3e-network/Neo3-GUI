using System.Numerics;
using Neo.Cryptography.ECC;

namespace Neo.Services.Abstractions
{
    /// <summary>
    /// Candidate service interface
    /// </summary>
    public interface ICandidateService
    {
        (ECPoint PublicKey, BigInteger Votes)[] GetCandidates();
    }
}
