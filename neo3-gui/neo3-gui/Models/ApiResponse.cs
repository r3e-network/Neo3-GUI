namespace Neo.Models
{
    /// <summary>
    /// Standard API response wrapper
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public WsError Error { get; set; }

        public static ApiResponse<T> Ok(T data) => 
            new() { Success = true, Data = data };

        public static ApiResponse<T> Fail(WsError error) => 
            new() { Success = false, Error = error };
    }
}
