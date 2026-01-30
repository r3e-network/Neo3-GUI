using Neo.Models;

namespace Neo.Exceptions
{
    /// <summary>
    /// Exception for validation errors
    /// </summary>
    public class ValidationException : NeoGuiException
    {
        public string ParameterName { get; }

        public ValidationException(string parameterName, string message)
            : base(ErrorCode.InvalidPara, message)
        {
            ParameterName = parameterName;
        }
    }

    public class ParameterNullException : ValidationException
    {
        public ParameterNullException(string parameterName)
            : base(parameterName, $"{parameterName} cannot be null or empty")
        {
        }
    }
}
