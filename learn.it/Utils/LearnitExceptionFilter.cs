using learn.it.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace learn.it.Utils
{
    public class LearnitExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var ex = context.Exception;
            switch (ex)
            {
                case UserNotFoundException:
                case GroupJoinRequestNotFoundException:
                case GroupNotFoundException:
                case StudySetNotFoundException:
                case FlashcardNotFoundException:
                    context.Result = new NotFoundObjectResult(new
                    {
                        Message = ex.Message
                    });
                    break;
                case EmailExistsException:
                case GroupJoinRequestExistsException:
                case UsernameExistsException:
                    context.Result = new ConflictObjectResult(new
                    {
                        Message = ex.Message
                    });
                    break;
                case InvalidInputDataException:
                    context.Result = new BadRequestObjectResult(new
                    {
                        Message = ex.Message
                    });
                    break;
                default:
                    return;
            }
            context.ExceptionHandled = true;
        }
    }
}
