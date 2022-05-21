using KLTN.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using WebAPI.Models;

namespace WebAPI.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var exception = new CustomException(context.Exception);
            var response = new FailResponseModel(exception);
            context.Result = new JsonResult(response)
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}
