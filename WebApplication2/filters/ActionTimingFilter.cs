using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace WebApplication2.filters
{
    public class ActionTimingFilter(ILogger<ActionTimingFilter> logger) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await next();

            }
            finally
            {
                stopwatch.Stop();

                logger.LogInformation(
                    "Action {ActionName} executed in {ElapsedMilliseconds} ms",
                    context.ActionDescriptor.DisplayName,
                    stopwatch.Elapsed.TotalMilliseconds);
            }

        }
    }
}
