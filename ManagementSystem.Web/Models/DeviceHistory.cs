namespace ManagementSystem.Web.Models
{
    public class DeviceHistory
    {
        public DeviceDto? Device { get; set; }
        public IEnumerable<DeviceLoanDto>? DeviceLoans { get; set; }
    }
}
