using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebTruss.Response
{
    public static class ControllerExtensions
    {
        public static IActionResult OkResult(this ControllerBase controller, object data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data cannot be null.");
            }

            return controller.Ok(ApiResponse<object>.Success(data));
        }

        public static IActionResult OkResult(this ControllerBase controller, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException("message cannot be null or empty.");
            }

            return controller.Ok(ApiResponse<object>.Success(message));
        }

        public static IActionResult OkResult(this ControllerBase controller, object data, string message)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException("message cannot be null or empty.");
            }

            return controller.Ok(ApiResponse<object>.Success(data, message));
        }

        public static IActionResult BadRequestResult(this ControllerBase controller, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new ArgumentNullException("errorMessage cannot be null or empty.");
            }

            return controller.BadRequest(ApiResponse<object>.Failed(errorMessage, HttpStatusCode.BadRequest));
        }

        public static IActionResult NotFoundResult(this ControllerBase controller, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new ArgumentNullException("errorMessage cannot be null or empty.");
            }

            return controller.NotFound(ApiResponse<object>.Failed(errorMessage, HttpStatusCode.NotFound));
        }

        public static IActionResult InternalServerErrorResult(this ControllerBase controller, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new ArgumentNullException("errorMessage cannot be null or empty.");
            }

            return controller.StatusCode(Convert.ToInt32(HttpStatusCode.InternalServerError), ApiResponse<object>.Failed(errorMessage, HttpStatusCode.InternalServerError));
        }
    }
}
