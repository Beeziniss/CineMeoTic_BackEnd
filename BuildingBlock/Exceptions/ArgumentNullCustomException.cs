namespace BuildingBlocks.Exceptions;
public sealed class ArgumentNullCustomException(string message) : BaseException(message)
{
    public override int StatusCode => 400; // Default status code is 400
    public override string ErrorCode => "AN"; // Custom error type for argument null exceptions
}
