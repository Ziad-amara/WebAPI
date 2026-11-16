using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Errors
{
    public static class ExceptionProblemDetailsFactory
    {
        public static ProblemDetails Create(Exception exception, HttpContext context, IHostEnvironment environment)
        {
            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occured.",
                Detail = environment.IsDevelopment() ? exception.Message : "please try again later.",
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = context.TraceIdentifier;
            return problem;
        }
    }
}
