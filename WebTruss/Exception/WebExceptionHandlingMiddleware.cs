using System.Diagnostics;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using WebTruss.Logging;
using System.Net;
using WebTruss.Extensions;

namespace WebTruss.Exception
{
    public class WebExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public WebExceptionHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, IWebLogger logger)
        {
            try
            {
                await next(context);
            }
            catch (WebException exception)
            {
                var response = WebApiResponse<object>.Failed(exception.Message, exception.HttpStatusCode);
                await HandleExceptionAsync(context, response, exception, logger, exception.HttpStatusCode);
            }
            catch (System.Exception exception)
            {
                var response = WebApiResponse<object>.Failed(
                    $"Something went wrong. To help us resolve the issue please keep the 'Reference Id' handy. Reference Id: {Activity.Current?.Id}",
                    HttpStatusCode.InternalServerError);
                await HandleExceptionAsync(context, response, exception, logger, HttpStatusCode.InternalServerError);
                logger.Log("error.");
                logger.Log($"Activity Id: {Activity.Current?.Id}");
                logger.Log(exception.UnwrapExceptionMessages());
            }
        }
        private static Task HandleExceptionAsync(
            HttpContext context, WebApiResponse<object> response, System.Exception exception, IWebLogger logger, HttpStatusCode httpStatusCode)
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
