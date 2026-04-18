namespace BuildingBlocks.Exceptions;

public sealed class NotFoundCustomException(string message) : BaseException(message)
{
    public override int StatusCode => 500; // Default status code for not found errors
    public override string ErrorCode => "NF"; // Custom error type for not found errors
}
