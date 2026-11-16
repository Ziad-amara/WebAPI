using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication2.filters
{
    public class ActionExecutionLoggingFilter( ILogger<ActionExecutionLoggingFilter> logger) : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var actionName = context.ActionDescriptor.DisplayName;
            if(context.Exception is not null && context.ExceptionHandled)
            {
                logger.LogError(
                       context.Exception,
                        "Action {ActionName} failed",
                        actionName);

                return;
            }
            
            logger.LogInformation(
                "Action {ActionName} executed successfully",
                actionName);
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            logger.LogInformation(
                "Starting execution of action {ActionName}",
                context.ActionDescriptor.DisplayName);
        }
    }
}
