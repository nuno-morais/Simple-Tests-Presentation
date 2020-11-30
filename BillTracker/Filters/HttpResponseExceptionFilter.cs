using BillTracker.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BillTracker.Filters
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is EntityNotFoundException)
            {
                context.Result = new NotFoundResult();
                context.ExceptionHandled = true;
            } else if (context.Exception is BadRequestException)
            {
                context.Result = new BadRequestResult();
                context.ExceptionHandled = true;
            }
        }
    }
}
