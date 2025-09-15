namespace learnyx.Models.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public object Errors { get; set; }
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; }

    public ApiResponse()
    {
        Timestamp = DateTime.UtcNow;
    }

    public ApiResponse(bool success, string message, T data = default, object errors = null, int statusCode = 200)
    {
        Success = success;
        Message = message;
        Data = data;
        Errors = errors;
        StatusCode = statusCode;
        Timestamp = DateTime.UtcNow;
    }
}

public class ApiResponse : ApiResponse<object>
{
    public ApiResponse() : base() { }
        
    public ApiResponse(bool success, string message, object data = null, object errors = null, int statusCode = 200)
        : base(success, message, data, errors, statusCode) { }
}