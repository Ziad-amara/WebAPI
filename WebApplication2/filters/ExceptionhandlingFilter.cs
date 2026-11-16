using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApplication2.Errors;

namespace WebApplication2.filters
{
    public class ExceptionhandlingFilter(ILogger<ExceptionhandlingFilter> logger, IHostEnvironment environment) : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled)
                return;

            logger.LogInformation(
                context.Exception,
                "An unhandled exception occurred while executing action {ActionName} for request {traceId}",
                context.ActionDescriptor.DisplayName,
                context.HttpContext.TraceIdentifier);

            var problem = ExceptionProblemDetailsFactory.Create(
                context.Exception,
                context.HttpContext,
                environment);

            var result = new ObjectResult(problem)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            result.ContentTypes.Add("application/problem+json");

            context.Result = result;
            context.ExceptionHandled = true;

        }
    }
}
