using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Shared.Helpers.CustomisedAttribute;

public class NotFutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is DateTime dateValue)
        {
            return dateValue <= DateTime.Now;
        }
        return true; // Non-datetime values are considered valid.
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} cannot be a future date.";
    }
}
