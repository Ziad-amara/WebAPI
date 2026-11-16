namespace WebApplication2.MiddleWares
{
    public sealed class RequestIdHeaderMiddleware(ILogger<RequestIdHeaderMiddleware> logger) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var requestId = context.TraceIdentifier;
            context.Response.Headers["X-Request-ID"] = requestId;

            logger.LogDebug(
                "Assigned Request ID {RequestId} to {Method} {Path}",
                requestId,
                context.Request.Method,
                context.Request.Path
                );

            await next(context);
        }
    }
}
