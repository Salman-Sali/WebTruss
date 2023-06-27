using System.Net;
using System.Text.Json.Serialization;

namespace WebTruss.Response
{
    public class ApiResponse<TData>
    {
        public ApiResponse()
        {

        }

        [JsonConstructor]
        public ApiResponse(string message, TData data)
        {
            Message = message;
            Data = data;
        }

        [JsonPropertyName("message")]
        public string Message { get; set; }


        [JsonPropertyName("data")]
        public TData Data { get; set; }

        public static ApiResponse<TData> Success(TData data, string message)
        {
            return new ApiResponse<TData>
            {
                Data = data,
                Message = message
            };
        }

        public static ApiResponse<TData> Success(string message)
        {
            return new ApiResponse<TData>
            {
                Data = default,
                Message = message
            };
        }


        public static ApiResponse<TData> Success(TData data)
        {
            return new ApiResponse<TData>
            {
                Data = data,
                Message = ""
            };
        }

        public static ApiResponse<TData> Failed(string message, HttpStatusCode httpStatusCode)
        {
            return new ApiResponse<TData>
            {
                Data = default,
                Message = message
            };
        }
    }
}
