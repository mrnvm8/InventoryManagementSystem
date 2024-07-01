using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Shared.Requests.Authentication
{
    public class UserRequest : AuthBaseCode
    {
        [DataType(DataType.Password),
        Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public required string ConfirmPassword { get; init; }
    }
}
