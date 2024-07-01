namespace ManagementSystem.Shared.Helpers
{
    public class EmailSettings
    {
        public required string EmailUsername { get; init; }
        public required string EmailPassword { get; init; }
        public required string EmailHost { get; init; }
    }
}
