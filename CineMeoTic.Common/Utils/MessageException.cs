namespace CineMeoTic.Common.Utils;

public static class MessageException
{
    public const string UserNotFound = "User not found.";
    public const string InvalidCredential = "Invalid email or password.";
    public const string EmailRequired = "Email is required.";
    public const string PasswordRequired = "Password is required.";
    public const string EmailAlreadyExists = "Email is already registered.";
    public const string Unauthorized = "Unauthorized access.";
    public const string Forbidden = "Forbidden access.";
    public const string EmailInvalidFormat = "Invalid email format.";
    public const string PasswordInvalidFormat = "Password must contain at least one lowercase letter, one uppercase letter, one digit, and be at least 6 characters long.";
    public const string DisplayNameRequired = "DisplayName is required.";
    public const string DisplayMinNameLength = "DisplayName must be at least 2 characters long.";
    public const string DisplayMaxNameLength = "DisplayName must be at most 100 characters long.";
}
