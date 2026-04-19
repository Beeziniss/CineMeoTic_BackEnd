namespace BuildingBlocks.Exceptions.Handler;

public sealed class UnAuthenticatedCustomException(string message) : BaseException(message)
{
    public override int StatusCode => 401;
    public override string ErrorCode => "UA"; // Custom error type for unauthenticated access
}
