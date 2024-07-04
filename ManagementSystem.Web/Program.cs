global using ManagementSystem.Application.Entities;
global using ManagementSystem.Application.Services.Interface;
global using ManagementSystem.Shared.Enums;
global using ManagementSystem.Shared.Requests;
global using ManagementSystem.Shared.Requests.Authentication;
global using ManagementSystem.Shared.Responses.ApplicationDTOs;
using ManagementSystem.Web;
using ManagementSystem.Web.Helpers;
using ManagementSystem.Web.Middlewares;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddApplicationSevices(builder);
}


var app = builder.Build();
{
    // Configure the localization options
    app.UseRequestLocalization(new RequestLocalizationOptions
    {
        DefaultRequestCulture = new RequestCulture("en-ZA"),
        SupportedCultures = new[] { new CultureInfo("en-ZA") },
        SupportedUICultures = new[] { new CultureInfo("en-ZA") },
    });

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    // Add custom exception handling middleware
    app.UseMiddleware<CheckAuthenticationMiddleware>();

    app.MapControllerRoute(
        name: "login",
        pattern: "Users/Login",
        defaults: new { controller = "Users", action = "Login" });

    app.MapControllerRoute(
        name: "registration",
        pattern: "Users/Registration",
        defaults: new { controller = "Users", action = "Registration" });

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .RequireAuthorization(AuthenticationConstants.LoggedInPolicyName);

    app.Run();
}




