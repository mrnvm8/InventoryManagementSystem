global using ManagementSystem.Application.Entities;
global using ManagementSystem.Application.Services.Interface;
global using ManagementSystem.Shared.Enums;
global using ManagementSystem.Shared.Requests;
global using ManagementSystem.Shared.Requests.Authentication;
global using ManagementSystem.Shared.Responses.ApplicationDTOs;
using ManagementSystem.Web;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddApplicationSevices(builder);
}

var app = builder.BuildAndConfigure();
{
    app.Run();
}




