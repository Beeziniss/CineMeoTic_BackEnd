namespace BuildingBlocks.Exceptions;

public sealed class BadRequestCustomException(string message) : BaseException(message)
{
    public override int StatusCode => 400; // Default status code is 400
    public override string ErrorCode => "BR"; // Custom error type for bad requests
}
