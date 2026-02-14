using System.Net;
using System.Text.Json;
using BuildingBlocks.Exceptions;
using CineMeoTic.Common.Utils;
using Serilog;

namespace CineMeoTic.UserService.API.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next)//, ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    //private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpResponse response = context.Response;
        response.ContentType = "application/json";

        ErrorResponse errorResponse = new();

        switch (exception)
        {
            case BaseException baseException:
                response.StatusCode = baseException.StatusCode;
                errorResponse.StatusCode = baseException.StatusCode;
                errorResponse.Message = baseException.Message;
                errorResponse.ErrorType = baseException.ErrorType;
                Log.Error(baseException, "Custom exception occurred: {ErrorType} - {Message}",
                    baseException.ErrorType, baseException.Message);
                break;

            case ArgumentNullException argumentNullException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = argumentNullException.Message;
                errorResponse.ErrorType = "ArgumentNull";
                Log.Warning(argumentNullException, "Argument null exception: {Message}",
                    argumentNullException.Message);
                break;

            case ArgumentException argumentException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = argumentException.Message;
                errorResponse.ErrorType = "InvalidArgument";
                Log.Warning(argumentException, "Argument exception: {Message}",
                    argumentException.Message);
                break;

            case UnauthorizedAccessException unauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "Unauthorized access";
                errorResponse.ErrorType = "Unauthorized";
                Log.Warning(unauthorizedAccessException, "Unauthorized access attempt");
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "An internal server error occurred";
                errorResponse.ErrorType = "InternalServerError";
                Log.Error(exception, "Unhandled exception occurred: {Message}", exception.Message);
                break;
        }

        errorResponse.TraceId = context.TraceIdentifier;
        errorResponse.Path = context.Request.Path;
        errorResponse.Timestamp = CustomTimeProvider.GetUtcPlus7Time();

        JsonSerializerOptions jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        string result = JsonSerializer.Serialize(errorResponse, jsonOptions);
        await response.WriteAsync(result);
    }

    private class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ErrorType { get; set; } = string.Empty;
        public string TraceId { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = CustomTimeProvider.GetUtcPlus7Time();
    }
}
