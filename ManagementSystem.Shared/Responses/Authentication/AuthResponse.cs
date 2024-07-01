namespace ManagementSystem.Shared.Responses.Authentication;

public class AuthResponse
{
    public Guid? Id { get; set; }
    public required string Token { get; set; }
}