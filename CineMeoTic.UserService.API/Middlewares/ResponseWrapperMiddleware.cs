using BuildingBlocks.Behaviors;

namespace CineMeoTic.UserService.API.Middlewares;

public class ResponseWrapperMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await _next(context);

            return;
        }

        // Only wrap API responses, skip for other paths (like static files)

        Stream originalBody = context.Response.Body;

        await using MemoryStream newBody = new();
        context.Response.Body = newBody;

        await _next(context);

        newBody.Seek(0, SeekOrigin.Begin);
        string bodyText = await new StreamReader(newBody).ReadToEndAsync();

        ApiResponse<object> response = new()
        {
            Success = context.Response.StatusCode < 400,
            Data = bodyText,
            Timestamp = DateTimeOffset.UtcNow,
        };

        string json = System.Text.Json.JsonSerializer.Serialize(response);

        context.Response.Body = originalBody;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(json);
    }
}
