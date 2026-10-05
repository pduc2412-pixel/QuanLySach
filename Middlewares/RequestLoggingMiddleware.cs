using System.Diagnostics;

namespace QuanLySach.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var method = context.Request.Method;
        var path = context.Request.Path.ToString();

        Console.WriteLine($"[{time}] Method: {method} - Path: {path}");

        if (IsInvalidBookId(context.Request.Path))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "text/plain; charset=utf-8";
            await context.Response.WriteAsync("Book id không hợp lệ");
            LogStatus(context, stopwatch);
            return;
        }

        await _next(context);

        LogStatus(context, stopwatch);
    }

    private static void LogStatus(HttpContext context, Stopwatch stopwatch)
    {
        stopwatch.Stop();
        Console.WriteLine($"Status Code: {context.Response.StatusCode} - {stopwatch.ElapsedMilliseconds} ms");
    }

    private static bool IsInvalidBookId(PathString path)
    {
        var segments = path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        if (segments.Length < 3) return false;

        var isBookController = segments[0].Equals("Sach", StringComparison.OrdinalIgnoreCase)
            || segments[0].Equals("Book", StringComparison.OrdinalIgnoreCase);
        var isDetailAction = segments[1].Equals("Details", StringComparison.OrdinalIgnoreCase)
            || segments[1].Equals("Detail", StringComparison.OrdinalIgnoreCase);

        return isBookController && isDetailAction && int.TryParse(segments[2], out var id) && id <= 0;
    }
}   