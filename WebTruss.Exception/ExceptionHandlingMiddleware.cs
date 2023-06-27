using System.Diagnostics;
using System.Net.Http;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using WebTruss.Response;
using System.Text;

namespace WebTruss.Exceptions
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IExceptionLogger logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, IExceptionLogger logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (AppException exception)
            {
                var response = ApiResponse<object>.Failed(exception.FriendlyMessage, exception.HttpStatusCode);
                await HandleExceptionAsync(context, response, exception, logger, exception.HttpStatusCode);
            }
            catch (Exception exception)
            {
                var response = ApiResponse<object>.Failed(
                    $"Something went wrong. To help us resolve the issue please keep the 'Reference Id' handy. Reference Id: {Activity.Current?.Id}",
                    HttpStatusCode.InternalServerError);
                await HandleExceptionAsync(context, response, exception, logger, HttpStatusCode.InternalServerError);
                logger.Log("error.");
                logger.Log($"Activity Id: {Activity.Current?.Id}");
                logger.Log(exception.UnwrapExceptionMessages());
                logger.Log(exception.InnerException?.UnwrapExceptionMessages() ?? "");
            }
        }

        private static Task HandleExceptionAsync(
            HttpContext context, ApiResponse<object> response, Exception exception, IExceptionLogger logger, HttpStatusCode httpStatusCode)
        {


            logger.Log("****************************** API ERROR ******************************");
            logger.Log($"Activity Id: {Activity.Current?.Id}");
            logger.Log(exception.UnwrapExceptionMessages());
            var result =
                JsonSerializer.Serialize(
                    response,
                    MatchStyleOfExistingMvcProblemDetails());

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ((int)httpStatusCode);

            return context.Response.WriteAsync(result);

            static JsonSerializerOptions MatchStyleOfExistingMvcProblemDetails()
            {
                return new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.Never
                };
            }
        }

        
    }
}
