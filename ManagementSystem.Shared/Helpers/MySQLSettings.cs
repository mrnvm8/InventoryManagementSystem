namespace ManagementSystem.Shared.Helpers;

public class MySQLSettings
{
    public required string Server { get; init; }
    public required string User { get; init; }
    public required string Database { get; init; }
    public required string Password { get; init; }
    public string ConnectionString
    {
        get
        {
            return $"server={Server};user={User};database={Database};password={Password}";
        }
    }
}