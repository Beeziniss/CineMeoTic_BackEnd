namespace BuildingBlocks.Utils;

public static class MessageException
{
    #region Required
    public const string DisplayNameRequired = "DisplayName is required.";
    public const string NameRequired = "Name is required.";
    public const string EmailRequired = "Email is required.";
    public const string PasswordRequired = "Password is required.";
    public const string RoleRequired = "Role is required.";
    public const string PermissionRequired = "Permission is required.";
    #endregion

    #region Invalid Format
    public const string InvalidCredential = "Invalid email or password.";
    public const string InvalidEmailFormat = "Invalid email format.";
    public const string InvalidPasswordFormat = "Password must contain at least one lowercase letter, one uppercase letter, one digit, and be at least 6 characters long.";
    public const string InvalidPermissionFormat = "Permission name must be in the format of uppercase letters and dots.";
    #endregion

    #region Length Constraints
    public const string DisplayNameMinLength = "DisplayName must be at least 2 characters long.";
    public const string DisplayNameMaxLength = "DisplayName must be at most 100 characters long.";
    #endregion

    #region Existence
    public const string EmailAlreadyExists = "Email is already registered.";
    public const string PermissionAlreadyExists = "Permission already exists.";
    public const string RoleAlreadyExists = "Role already exists.";
    public const string PermissionsAlreadyExist = "One or more permissions already exist.";
    #endregion

    #region Not Found
    public const string UserNotFound = "User not found.";
    public const string ViewerNotFound = "Viewer not found.";
    public const string PermissionNotFound = "Permission not found.";
    public static string PermissionNotFoundMethod(string permissionName) => $"Permission {permissionName} not found";
    #endregion

    public const string UnAuthenticated = "Unauthenticated access.";
    public const string Unauthorized = "Unauthorized access.";
    public const string Forbidden = "Forbidden access.";
}
