using learn.it.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace learn.it.Utils
{
    public class UserNotFoundExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is not UserNotFoundException userNotFoundException) return;
            context.Result = new NotFoundObjectResult(new
            {
                Message = userNotFoundException.Message
            });
            context.ExceptionHandled = true;
        }
    }
}
