namespace ManagementSystem.Web.Helpers;

public class ControllersErrors
{

    public const string Resubmit = "Please correct the errors and resubmit.";
    public string InvalidID(string itemName)
    {
        return $"Invalid {itemName} Id";
    }

}
