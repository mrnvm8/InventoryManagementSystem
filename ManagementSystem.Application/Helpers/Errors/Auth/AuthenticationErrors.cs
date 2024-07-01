namespace ManagementSystem.Application.Helpers.Errors.Auth;

public static class AuthSuccessMessage
{
    //Success messages
    public const string CreateSuccess = "Registration was successful.";
    public const string LogniSuccess = "Login was successful.";
}

public static class AuthErrorMessage
{
    //Failed messages
    public const string IncorrectError = "Incorrect credentials";
    public const string CreateFailError = "Failed to create the user";
    public const string DeleteError = "Fail to delete the user";
    public const string UpdateError = "Fail to change the user password";
    public const string NotFoundError = "User does not exist in the application";
    public const string UserExist = "User already exist.";
    public const string NoUsers = "No users registered yet";
    public const string Fobidden = "User is fobidden to register";
}
