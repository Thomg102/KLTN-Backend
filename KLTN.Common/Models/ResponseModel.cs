using KLTN.Common.Exceptions;
using System.Net;
using WebAPI.Models;

namespace WebAPI.Models
{
    public class SuccessResponseModel {
        public object Result { get; set; }

        public SuccessResponseModel()
        {
            Result = new object();
        }

        public SuccessResponseModel(object result)
        {
            Result = result;
        }
    }

    public class FailResponseModel
    {
        public ErrorResponseModel Error { get; set; }

        public FailResponseModel()
        {
        }

        public FailResponseModel(ErrorResponseModel error)
        {
            Error = error;
        }
        public FailResponseModel(CustomException exception)
        {
            Error = new ErrorResponseModel()
            {
                Code = exception.ErrorCode,
                Message = exception.Message,
            };

        }
    }

    public class ErrorResponseModel
    {
        public string Message { get; set; }
        public int Code { get; set; }
    }
}