namespace LibraryManagementSystem.Middlewares;

public class LoggingMiddleware : IMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var request = context.Request;

        _logger.LogInformation("Incoming request: {Method} {Path} | Query: {QueryString} | Headers: {Headers}",
            request.Method, request.Path, request.QueryString,
            request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        await next(context);

        stopwatch.Stop();

        _logger.LogInformation("Response send: {Method} {Path} | Status: {StatusCode} | Time: {Elapsed}ms",
            request.Method, request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}