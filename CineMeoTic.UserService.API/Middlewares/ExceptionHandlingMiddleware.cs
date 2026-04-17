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
            case FluentValidation.ValidationException validationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Validation failed";
                errorResponse.ErrorType = "ValidationError";

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
                errorResponse.ErrorType = baseException.ErrorType;
                Log.Error(baseException, "Custom exception occurred: {ErrorType} - {Message}",
                    baseException.ErrorType, baseException.Message);
                break;

            case InvalidOperationException invalidOperationException when IsTransientDatabaseFailure(invalidOperationException):
                response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                errorResponse.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                errorResponse.Message = "Database is temporarily unavailable. Please try again.";
                errorResponse.ErrorType = "DatabaseUnavailable";
                Log.Error(invalidOperationException, "Transient database failure occurred: {Message}",
                    invalidOperationException.Message);
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
        public string ErrorType { get; set; } = string.Empty;
        public Dictionary<string, string[]>? Errors { get; set; }
        public string TraceId { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = CustomTimeProvider.GetUtcPlus7Time();
    }
}
