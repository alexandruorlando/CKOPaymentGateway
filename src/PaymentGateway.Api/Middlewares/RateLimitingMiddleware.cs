namespace PaymentGateway.Api.Middlewares;

public class RateLimitingMiddleware(RequestDelegate next, int requestLimit = 100, int timeWindowInSeconds = 60)
{
    private static readonly Dictionary<string, (DateTime lastRequestTime, int requestCount)> RequestCounts = new();
    private readonly TimeSpan _timeSpan = TimeSpan.FromSeconds(timeWindowInSeconds);

    public Task InvokeAsync(HttpContext context)
    {
        var key = context.Connection.RemoteIpAddress?.ToString() ?? context.Request.Headers["Session-ID"].ToString();
        var currentTime = DateTime.UtcNow;

        if (RequestCounts.ContainsKey(key))
        {
            var (lastRequestTime, requestCount) = RequestCounts[key];
            if (currentTime - lastRequestTime < _timeSpan)
            {
                if (requestCount >= requestLimit)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    return Task.CompletedTask;
                }

                RequestCounts[key] = (lastRequestTime, requestCount + 1);
            }
            else
            {
                RequestCounts[key] = (currentTime, 1);
            }
        }
        else
        {
            RequestCounts[key] = (currentTime, 1);
        }

        return next(context);
    }
}