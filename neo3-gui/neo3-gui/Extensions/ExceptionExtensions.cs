namespace Neo.Extensions
{
    /// <summary>
    /// Exception extension methods
    /// </summary>
    public static class ExceptionExtensions
    {
        public static string GetExMessage(this Exception ex)
        {
            var msg = ex.Message;
            while (ex.InnerException != null)
            {
                msg += $"\r\n----[{ex.InnerException.Message}]";
                ex = ex.InnerException;
            }
            return msg;
        }
    }
}
