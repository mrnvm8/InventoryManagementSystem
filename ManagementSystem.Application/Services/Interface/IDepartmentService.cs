using ManagementSystem.Shared.Requests;
using ManagementSystem.Shared.Responses.ApplicationDTOs;

namespace ManagementSystem.Application.Services.Interface;
public interface IDepartmentService : IGenericService<DepartmentRequest, DepartmentDto> { }