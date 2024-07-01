using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Shared.Helpers.CustomisedAttribute;

public class AllowedEmailDomainAttribute : ValidationAttribute
{
    //field to hold param domains
    private readonly string _domain;

    public AllowedEmailDomainAttribute(string domain)
    {
        _domain = domain;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        //check if the value is not null
        //if null return error message requesting email;
        if (value != null)
        {
            //get the email
            var email = value.ToString();
            //get the email domain after the @ symbol
            var emailDomain = email!.Substring(email.IndexOf('@') + 1);

            //compare the domain of the email with the system domain
            //give an error message if the domain does not match any of system domain
            if (_domain.Equals(emailDomain, StringComparison.OrdinalIgnoreCase))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult($"Email domain must be one of the following: {string.Join(", ", _domain)}");
            }
        }

        return new ValidationResult("Email is required.");
    }
}
