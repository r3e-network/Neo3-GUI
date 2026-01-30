namespace Neo.Models.Wallets
{
    /// <summary>
    /// Transfer request with validation
    /// </summary>
    public class TransferRequestV2 : RequestBase
    {
        public UInt160 Receiver { get; set; }
        public string Amount { get; set; }
        public string Asset { get; set; } = "neo";
        public UInt160 Sender { get; set; }

        public override bool IsValid(out string error)
        {
            error = null;
            if (Receiver == null)
            {
                error = "receiver is required";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Amount))
            {
                error = "amount is required";
                return false;
            }
            return true;
        }
    }
}
