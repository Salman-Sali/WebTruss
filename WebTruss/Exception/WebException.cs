using System.Net;

namespace WebTruss.Exception
{
    public class WebException : ApplicationException
    {
        public string Message { get; }
        public HttpStatusCode HttpStatusCode { get; }

        public WebException(string message, HttpStatusCode httpStatusCode) : base(message)
        {
            Message = message;
            HttpStatusCode = httpStatusCode;
        }
    }
}
