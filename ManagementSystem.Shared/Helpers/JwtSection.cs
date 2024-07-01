namespace ManagementSystem.Shared.Helpers;
public class JwtSection
{
    public const string SectionName = "JwtSection";
    public string? Key { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public int ExpiryMinutes { get; set; }
}
