namespace WebApplication2.MiddleWares
{
    public sealed class RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        private static int _counter = 0;
        private static DateTime _lastRequestDate = DateTime.UtcNow;
        public async Task InvokeAsync(HttpContext context)
        {
            _counter++;
            
            if(DateTime.Now.Subtract(_lastRequestDate).TotalSeconds > 10)
            {
                _counter = 1;
                _lastRequestDate = DateTime.UtcNow;
                await next(context);
            }
            else
            {
                if (_counter > 15)
                {
                    _lastRequestDate = DateTime.UtcNow;
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync("Too many requests. Please try again later.");
                }
                else
                {
                    _lastRequestDate = DateTime.UtcNow;
                    await next(context);
                }
            }
        }
    }
}
