using ManagementSystem.Shared.Helpers.CustomisedAttribute;
using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Shared.Requests.Authentication
{
    public class AuthBaseCode
    {
        [Required(ErrorMessage = "Your Work email is required"),
        StringLength(60, MinimumLength = 3),
        EmailAddress,
        Display(Name = "Username"),
        AllowedEmailDomain("axiumeducation.org")]
        public required string Email { get; init; }

        [DataType(DataType.Password),
        Required(ErrorMessage = "Password is required"),
        StringLength(120, MinimumLength = 8)]
        public required string Password { get; init; }
    }
}
