
namespace UserSystem.API.Models.Response
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public static ApiResponse FailMessage(string errorMessage)
        {
            return new ApiResponse { Success = false, Message = errorMessage };
        }
        public static ApiResponse SuccessMessage(string message)
        {
            return new ApiResponse { Success = true, Message = message };
        }
        public static ApiResponse SuccessResponse(object data)
        {
            return new ApiResponse { Success = true, Data = data };
        }
        public static ApiResponse SuccessResponse(string message, object data)
        {
            return new ApiResponse { Success = true, Message = message, Data = data };
        }
    }
}
