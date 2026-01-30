namespace Neo.Models.Wallets
{
    /// <summary>
    /// Open wallet request
    /// </summary>
    public class OpenWalletRequest : RequestBase
    {
        public string Path { get; set; }
        public string Password { get; set; }

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
            return true;
        }
    }
}
