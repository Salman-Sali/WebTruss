using System.Net;

namespace WebTruss.Exceptions
{
    public class AppException : ApplicationException
    {
        public string SystemMessage { get; }
        public string FriendlyMessage { get; }
        public HttpStatusCode HttpStatusCode { get; }

        public AppException(string systemMessage, string friendlyMessage, HttpStatusCode httpStatusCode) : base(systemMessage)
        {
            FriendlyMessage = friendlyMessage;
            HttpStatusCode = httpStatusCode;
            SystemMessage = systemMessage;
        }

        public AppException(string friendlyMessage, HttpStatusCode httpStatusCode) : base(friendlyMessage)
        {
            FriendlyMessage = friendlyMessage;
            HttpStatusCode = httpStatusCode;
        }

        public static AppException InternalServerError(string message)
        {
            return new AppException(message, HttpStatusCode.InternalServerError);
        }

        public static AppException BadRequest(string message)
        {
            return new AppException(message, HttpStatusCode.BadRequest);
        }
    }
}
