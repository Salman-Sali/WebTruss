using System.Net;
using System.Text.Json.Serialization;

namespace WebTruss.Extensions
{
    public class WebApiResponse<TData>
    {
        public WebApiResponse()
        {

        }

        [JsonConstructor]
        public WebApiResponse(string message, TData data)
        {
            Message = message;
            Data = data;
        }

        [JsonPropertyName("message")]
        public string Message { get; set; }


        [JsonPropertyName("data")]
        public TData Data { get; set; }


        public static WebApiResponse<TData> Success(TData data, string message)
        {
            return new WebApiResponse<TData>
            {
                Data = data,
                Message = message
            };
        }

        public static WebApiResponse<TData> Success(string message)
        {
            return new WebApiResponse<TData>
            {
                Data = default,
                Message = message
            };
        }


        public static WebApiResponse<TData> Success(TData data)
        {
            return new WebApiResponse<TData>
            {
                Data = data,
                Message = ""
            };
        }

        public static WebApiResponse<TData> Failed(string message, HttpStatusCode httpStatusCode)
        {
            return new WebApiResponse<TData>
            {
                Data = default,
                Message = message
            };
        }
    }
}
