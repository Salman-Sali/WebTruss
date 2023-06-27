using System.Text;

namespace WebTruss.Exceptions
{
    public static class Extensions
    {
        public static string UnwrapExceptionMessages(this Exception exception)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(exception.Message))
            {
                sb.AppendLine(exception.Message);
                sb.AppendLine(exception.StackTrace);
            }

            if (exception.InnerException != null)
            {
                sb = GetInnerExceptionMessage(sb, exception.InnerException);
            }

            return sb.ToString().Trim();
        }

        private static StringBuilder GetInnerExceptionMessage(StringBuilder sb, Exception exception)
        {
            if (!string.IsNullOrWhiteSpace(exception.Message))
            {
                sb.AppendLine(exception.Message);
                sb.AppendLine(exception.StackTrace);
            }

            if (exception.InnerException != null)
            {
                sb = GetInnerExceptionMessage(sb, exception.InnerException);
            }
            return sb;
        }
    }
}
