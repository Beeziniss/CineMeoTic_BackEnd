namespace BuildingBlocks.Exceptions;

public class BaseException(string message) : Exception(message)
{
    public virtual int StatusCode { get; } = 500; // Default status code is 500
    public virtual string ErrorCode => "BaseException"; // Default error type
}
