namespace BuildingBlocks.Exceptions;

public sealed class ForbiddenCustomException(string message) : BaseException(message)
{
    public override int StatusCode => 403; // Default status code for forbidden access
    public override string ErrorCode => "FA"; // Custom error type for forbidden access
}
