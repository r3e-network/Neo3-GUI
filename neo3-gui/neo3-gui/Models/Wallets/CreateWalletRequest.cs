using Neo.Common;

namespace Neo.Models.Wallets
{
    /// <summary>
    /// Create wallet request
    /// </summary>
    public class CreateWalletRequest : RequestBase
    {
        public string Path { get; set; }
        public string Password { get; set; }
        public string PrivateKey { get; set; }

        public override bool IsValid(out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(Path))
            {
                error = "path is required";
                return false;
            }
            if (string.IsNullOrEmpty(Password))
            {
                error = "password is required";
                return false;
            }
            if (Password.Length < AppConstants.MinPasswordLength)
            {
                error = $"password must be at least {AppConstants.MinPasswordLength} characters";
                return false;
            }
            return true;
        }
    }
}
