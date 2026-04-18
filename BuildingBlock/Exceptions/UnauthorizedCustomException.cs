namespace BuildingBlocks.Exceptions;

public sealed class UnauthorizedCustomException(string message) : BaseException(message)
{
    public override int StatusCode => 401;
    public override string ErrorCode => "UA"; // Custom error type for unauthorized access
}
