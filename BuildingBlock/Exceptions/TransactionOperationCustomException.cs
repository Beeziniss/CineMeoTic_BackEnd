namespace BuildingBlocks.Exceptions;
public sealed class TransactionOperationCustomException(string message) : BaseException(message)
{
    public override int StatusCode => 500;
    public override string ErrorCode => "TOCE"; // Custom error type for invalid transaction operations
}
