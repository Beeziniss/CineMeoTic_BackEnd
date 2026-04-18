namespace BuildingBlocks.Exceptions;

public sealed class BadGatewayCustomException(string message) : BaseException(message)
{
    public override int StatusCode => 502; // Default status code for bad gateway
    public override string ErrorCode => "BG"; // Custom error type for bad gateway
}
