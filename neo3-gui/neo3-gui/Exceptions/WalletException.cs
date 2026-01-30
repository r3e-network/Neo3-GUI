using Neo.Models;

namespace Neo.Exceptions
{
    /// <summary>
    /// Exception for wallet-related errors
    /// </summary>
    public class WalletException : NeoGuiException
    {
        public WalletException(ErrorCode errorCode)
            : base(errorCode)
        {
        }

        public WalletException(ErrorCode errorCode, string message)
            : base(errorCode, message)
        {
        }
    }

    public class WalletNotOpenException : WalletException
    {
        public WalletNotOpenException()
            : base(ErrorCode.WalletNotOpen)
        {
        }
    }

    public class WalletFileNotFoundException : WalletException
    {
        public WalletFileNotFoundException(string path)
            : base(ErrorCode.WalletFileNotFound, $"Wallet file not found: {path}")
        {
        }
    }

    public class WrongPasswordException : WalletException
    {
        public WrongPasswordException()
            : base(ErrorCode.WrongPassword)
        {
        }
    }

    public class AddressNotFoundException : WalletException
    {
        public AddressNotFoundException(string address)
            : base(ErrorCode.AddressNotFound, $"Address not found: {address}")
        {
        }
    }

    public class InsufficientBalanceException : WalletException
    {
        public InsufficientBalanceException()
            : base(ErrorCode.BalanceNotEnough)
        {
        }

        public InsufficientBalanceException(string message)
            : base(ErrorCode.BalanceNotEnough, message)
        {
        }
    }

    public class InsufficientGasException : WalletException
    {
        public InsufficientGasException()
            : base(ErrorCode.GasNotEnough)
        {
        }
    }
}
