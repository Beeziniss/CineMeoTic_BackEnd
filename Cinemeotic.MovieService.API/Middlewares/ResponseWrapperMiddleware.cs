using BuildingBlocks.Utils;
using System.Text.Json;

namespace Cinemeotic.MovieService.API.Middlewares;

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
        
        context.Response.Body = originalBody;

        if (context.Response.StatusCode >= 400)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(bodyText);
            return;
        }

        object? data = null;
        if (!string.IsNullOrWhiteSpace(bodyText))
        {
            try
            {
                data = JsonSerializer.Deserialize<object>(bodyText);
            }
            catch
            {
                data = bodyText;
            }
        }

        object response = new
        {
            context.Response.StatusCode,
            Message = "Success",
            Data = data,
            TraceId = context.TraceIdentifier,
            Path = context.Request.Path.Value ?? string.Empty,
            Timestamp = CustomTimeProvider.GetUtcPlus7TimeOffset()
        };

        string json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(json);
    }
}
