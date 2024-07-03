using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace ManagementSystem.Web.Middlewares;

public class CheckAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CheckAuthenticationMiddleware> _logger;

    // This method is called for each HTTP request
    public CheckAuthenticationMiddleware(RequestDelegate next, ILogger<CheckAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Call the next middleware in the pipeline
            await _next(context);
        }
        catch (SecurityTokenExpiredException)
        {
            // Log the token expiration event
            _logger.LogInformation("Token expired. Redirecting to login page.");
            // Redirect the user to the login page
            context.Response.Redirect("/Users/Login");
        }
        catch (Exception ex)
        {
            // Log any unexpected errors
            _logger.LogError(ex, "An unexpected error occurred.");
            // Optionally rethrow the exception to let the default exception handler deal with it
            // throw;
            // Redirect to a generic error page
            await HandleExceptionAsync(context, ex);
           
        }

        // Check for 401 Unauthorized response
        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
        {
            _logger.LogInformation("Handling 401 response in middleware. Redirecting to login.");

            // Redirect to login page
            context.Response.Redirect("/Users/Login");
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "text/html";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // Redirect to a custom error page
        context.Response.Redirect("/Home/Error");
        // Log the exception for further analysis
       
        return Task.CompletedTask;
    }
}
