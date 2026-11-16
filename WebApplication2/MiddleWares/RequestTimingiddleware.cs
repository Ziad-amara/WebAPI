using System.Diagnostics;

namespace WebApplication2.MiddleWares
{
    public sealed class RequestTimingiddleware(RequestDelegate next, ILogger<RequestTimingiddleware> logger)
    {
      
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            Exception? Failure = null;

            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                Failure = ex;
                throw;
            }
            finally
            {
                stopwatch.Stop();
                var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                if (Failure != null)
                {
                    logger.LogError(
                        Failure,
                        "HTTP {Method} {Path} failed after {ElapsedMilliseconds} ms",
                        context.Request.Method,
                        context.Request.Path, 
                        elapsedMilliseconds
                        );
                }
                else
                {
                    logger.LogInformation(
                        "HTTP {Method} {Path} returned {StatusCode} in {ElapsedMilliseconds} ms",
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode,
                        elapsedMilliseconds
                        );
                }
            }
        }
    }
}
