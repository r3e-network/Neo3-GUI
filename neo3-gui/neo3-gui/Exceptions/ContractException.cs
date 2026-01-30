using Neo.Models;

namespace Neo.Exceptions
{
    /// <summary>
    /// Exception for contract-related errors
    /// </summary>
    public class ContractException : NeoGuiException
    {
        public ContractException(ErrorCode errorCode)
            : base(errorCode)
        {
        }

        public ContractException(ErrorCode errorCode, string message)
            : base(errorCode, message)
        {
        }
    }

    public class ContractNotFoundException : ContractException
    {
        public ContractNotFoundException(string hash)
            : base(ErrorCode.UnknownContract, $"Contract not found: {hash}")
        {
        }
    }

    public class ContractExecutionException : ContractException
    {
        public ContractExecutionException(string message)
            : base(ErrorCode.ExecuteContractFail, message)
        {
        }
    }

    public class InvalidContractException : ContractException
    {
        public InvalidContractException(string message)
            : base(ErrorCode.InvalidContractScript, message)
        {
        }
    }
}
