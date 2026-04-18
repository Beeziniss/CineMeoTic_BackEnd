namespace BuildingBlocks.Exceptions;

public sealed class ExternalServiceCustomException(string message) : BaseException(message)
{
    public override int StatusCode => 503; // Default status code for external service errors
    public override string ErrorCode => "ES"; // Custom error type for external service errors
}
