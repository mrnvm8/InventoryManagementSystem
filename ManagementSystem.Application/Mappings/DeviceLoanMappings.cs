using ManagementSystem.Application.Entities;
using ManagementSystem.Shared.Requests;
using ManagementSystem.Shared.Responses.ApplicationDTOs;

namespace ManagementSystem.Application.Mappings;

public static class DeviceLoanMappings
{
    //This method is for mapping LoanRequest for creation
    public static DeviceLoan MapToDeviceLoan(this LoanRequest request, Guid userId)
    {
        return new DeviceLoan
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DeviceId = request.DeviceId,
            EmployeeId = request.EmployeeId,
            AssignedDate = DateTime.Now,
            ReturnDate = DateTime.Parse("2009-01-01"),
            IsApproved = false,
        };
    }

    //This method is for mapping request to DeviceLoan to be updated
    public static DeviceLoan MapToDeviceLoanForUnassigned(this LoanRequest request, Guid deviceId, Guid userId, DeviceLoan deviceLoan)
    {
        return new DeviceLoan
        {
            Id = deviceLoan.Id,
            UserId = deviceLoan.UserId,
            DeviceId = deviceId,
            EmployeeId = deviceLoan.EmployeeId,
            AssignedDate = deviceLoan.AssignedDate,
            ReturnDate = DateTime.Now,
            ReturnToUserId = userId,
            IsApproved = true,
        };
    }

    //This method is for mapping DeviceLoan to DTO
    public static DeviceLoanDto MapToDeviceLoanDto(this DeviceLoan deviceLoans)
    {
        return new DeviceLoanDto
        {
            Id = deviceLoans.Id,
            DeviceId = deviceLoans.DeviceId,
            DepartmentId = deviceLoans.Device!.DepartmentId,
            DeviceTypeId = deviceLoans.Device!.DeviceTypeId,
            EmployeeId = deviceLoans.EmployeeId,
            AssignedDate = deviceLoans.AssignedDate,
            ReturnDate = deviceLoans.ReturnDate,
            IsApproved = deviceLoans.IsApproved,
            EmployeeName = $"{deviceLoans!.Employee?.Name}, " +
                          $"{deviceLoans!.Employee?.Surname}",
            //LoanDevice = deviceLoans.Device
        };
    }
}
