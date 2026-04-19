using System.Net;
using System.Text.Json;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Utils;
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

    // Error Codes:
    // VF: Validation Failure
    // DU: Database Unavailable
    // IS: Internal Server Error
    // VE: Validation Error
    // xCE: Custom Exception
    // AN: Argument Null Exception
    // BG: Bad Gateway
    // BR: Bad Request
    // C: Conflict
    // ES: External Service Error
    // FA: Forbidden Access
    // NF: Not Found
    // TO: Transient Operation Exception
    // UA: Unauthorized Access
    // UE01: Unconfigured Environment Exception
    // UE02: Unprocessable Entity
    // Custom error codes can be added as needed for different exception types
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpResponse response = context.Response;
        response.ContentType = "application/json";

        ErrorResponse errorResponse = new();

        switch (exception)
        {
            case FluentValidation.ValidationException validationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Validation failed";
                errorResponse.ErrorCode = "VF";

                Dictionary<string, string[]> errors = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                errorResponse.Errors = errors;

                Log.Warning(validationException, "Validation failed: {@Errors}", errors);
                break;

            case BaseException baseException:
                response.StatusCode = baseException.StatusCode;
                errorResponse.StatusCode = baseException.StatusCode;
                errorResponse.Message = baseException.Message;
                errorResponse.ErrorCode = baseException.ErrorCode;
                Log.Error(baseException, "Custom exception occurred: {ErrorCode} - {Message}",
                    baseException.ErrorCode, baseException.Message);
                break;

            case InvalidOperationException invalidOperationException when IsTransientDatabaseFailure(invalidOperationException):
                response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                errorResponse.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                errorResponse.Message = "Database is temporarily unavailable. Please try again.";
                errorResponse.ErrorCode = "DU";
                Log.Error(invalidOperationException, "Transient database failure occurred: {Message}",
                    invalidOperationException.Message);
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "An internal server error occurred";
                errorResponse.ErrorCode = "IS";
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

    private static bool IsTransientDatabaseFailure(Exception exception)
    {
        Exception? current = exception;

        while (current is not null)
        {
            if (current is TimeoutException)
            {
                return true;
            }

            current = current.InnerException;
        }

        return exception.Message.Contains("transient failure", StringComparison.OrdinalIgnoreCase)
            || exception.Message.Contains("Failed to connect", StringComparison.OrdinalIgnoreCase);
    }

    private class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public Dictionary<string, string[]>? Errors { get; set; }
        public string TraceId { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = CustomTimeProvider.GetUtcPlus7Time();
    }
}
