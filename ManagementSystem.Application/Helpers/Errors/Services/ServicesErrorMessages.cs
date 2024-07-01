namespace ManagementSystem.Application.Helpers.Errors.Services;

public class ServicesErrorMessages
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ServicesErrorMessages(Guid id) => Id = id;
    public ServicesErrorMessages(string name) { Name = name; }

    public const string TicketExist = "The same Ticket Issue exist on the system, Error duplication";
    public const string DeviceExist = "A device with the same Serial/IMEI number Exist";
    public const string EmailExist = "The email already exist on the system";
    public string IdNotFound
    { get{return $"The record with Id: {Id} does not exist.";}}

    public string Duplicate
    {get{return $"A same record with name: {Name.ToUpper()} exist in the database.";}}
}