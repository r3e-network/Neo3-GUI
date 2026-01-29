using System.Linq;
using System.Numerics;
using System.Text;
using Neo.Extensions;
using Neo.VM;

namespace Neo.Models.Contracts
{
    /// <summary>
    /// VM instruction information
    /// </summary>
    public class InstructionInfo
    {
        /// <summary>
        /// Byte position in script
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Operation code
        /// </summary>
        public OpCode OpCode { get; set; }

        /// <summary>
        /// OpCode name string
        /// </summary>
        public string OpCodeName => OpCode.ToString();

        /// <summary>
        /// Operation data bytes
        /// </summary>
        public byte[]? OpData { get; set; }

        /// <summary>
        /// System call method name (only for SYSCALL)
        /// </summary>
        public string? SystemCallMethod { get; set; }

        /// <summary>
        /// OpData as UTF8 string
        /// </summary>
        public string? OpDataUtf8String => 
            OpData == null ? null : Encoding.UTF8.GetString(OpData);

        /// <summary>
        /// Best-effort string representation of OpData
        /// </summary>
        public string? OpDataPossibleString => ToOpDataString();

        private string? ToOpDataString()
        {
            if (OpData == null) return null;
            if (SystemCallMethod.NotNull()) return SystemCallMethod;
            if (OpCode >= OpCode.PUSHINT8 && OpCode <= OpCode.PUSHINT256)
                return new BigInteger(OpData).ToString();
            var utf8String = OpDataUtf8String;
            if (utf8String != null && utf8String.Any(p => p < '0' || p > 'z'))
                return OpData.ToHexString();
            return utf8String;
        }
    }
}
