namespace Neo.Common
{
    /// <summary>
    /// Result pattern for operation outcomes
    /// </summary>
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public string Error { get; }
        public int ErrorCode { get; }

        private Result(T value)
        {
            IsSuccess = true;
            Value = value;
        }

        private Result(string error, int code)
        {
            IsSuccess = false;
            Error = error;
            ErrorCode = code;
        }

        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(string error, int code = 0) => new(error, code);
    }
}
