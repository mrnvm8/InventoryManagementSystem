namespace ManagementSystem.Web.Helpers;
public class AuthenticationConstants
{
    //Represent the Policy Name
    public const string EveryUserPolicyName = "EveryUser";

    public const string UserPolicyName = "User";
    public const string AdminPolicyName = "Admin";
    public const string AnonymousPolicyName = "RequireAnonymous";
    public const string LoggedInPolicyName = "RequireLoggedIn";

    //Represent the Role as the same a Database role values
    //This is saying that either the role is admin or user, this policy apply
    public const string AdminClaimName = "Admin";
    public const string UserClaimName = "User";

    /// <summary>
    /// Claims from Httpcpntent
    /// </summary>
    public const string AdminRole = "Admin";
    public const string EmployeeIdentifier = "EmployeeIdentifier";
    public const string DepartmentIdentifier = "DepartmentIdentifier";
}