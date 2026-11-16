using WebApplication2.Errors;

namespace WebApplication2.MiddleWares
{
    public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment environment)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {

                if(context.Response.HasStarted)
                {
                    logger.LogError(
                        ex,
                        "Request {TraceId} failed after response has started. the response cannot be replaced.",
                        context.TraceIdentifier);
                    throw;
                }

                logger.LogError(
                    ex,
                    "Unhandled exception occurred while the processing the request {TraceId}.",
                    context.TraceIdentifier
                );

                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.Headers["X-Request-ID"] = context.TraceIdentifier;

                var problem = ExceptionProblemDetailsFactory.Create(ex,context, environment);

                await context.Response.WriteAsJsonAsync(
                    problem,
                    options : null,
                    contentType : "application/problem+json",
                    cancellationToken : context.RequestAborted 
                    );
            }
        }
    }
}
