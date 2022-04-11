using System;
using System.Net;
using System.Security.Authentication;


namespace KLTN.Common.Exceptions
{
    public class CustomException : Exception
    {
        public int ErrorCode { get; set; }
        public object Result { get; set; }

        public CustomException()
        {
        }
        public CustomException(string message, int errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        public CustomException(string message, int errorCode, object result) : base(message)
        {
            ErrorCode = errorCode;
            Result = result;
        }

        public CustomException(Exception exception) : base(exception.Message, exception)
        {
            if (exception is AuthenticationException)
            {
                ErrorCode = (int)HttpStatusCode.Unauthorized;
            }
            else if (exception is UnauthorizedAccessException)
            {
                ErrorCode = (int)HttpStatusCode.Forbidden;
            }
            else if (exception is CustomException customException)
            {
                ErrorCode = customException.ErrorCode;

                Result = customException.Result;
            }
            else
            {
                ErrorCode = (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}
