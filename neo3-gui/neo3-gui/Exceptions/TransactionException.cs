using Neo.Models;

namespace Neo.Exceptions
{
    /// <summary>
    /// Exception for transaction-related errors
    /// </summary>
    public class TransactionException : NeoGuiException
    {
        public TransactionException(ErrorCode errorCode)
            : base(errorCode)
        {
        }

        public TransactionException(ErrorCode errorCode, string message)
            : base(errorCode, message)
        {
        }
    }

    public class TransactionNotFoundException : TransactionException
    {
        public TransactionNotFoundException(string txId)
            : base(ErrorCode.TxIdNotFound, $"Transaction not found: {txId}")
        {
        }
    }

    public class SignatureException : TransactionException
    {
        public string SignContext { get; }

        public SignatureException(string signContext)
            : base(ErrorCode.SignFail, "Transaction signing failed")
        {
            SignContext = signContext;
        }
    }

    public class TransactionSizeException : TransactionException
    {
        public TransactionSizeException()
            : base(ErrorCode.ExceedMaxTransactionSize)
        {
        }
    }
}
