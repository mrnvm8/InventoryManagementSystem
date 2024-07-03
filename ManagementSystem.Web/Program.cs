global using ManagementSystem.Application.Entities;
global using ManagementSystem.Application.Services.Interface;
global using ManagementSystem.Shared.Enums;
global using ManagementSystem.Shared.Requests;
global using ManagementSystem.Shared.Requests.Authentication;
global using ManagementSystem.Shared.Responses.ApplicationDTOs;
using ManagementSystem.Application;
using ManagementSystem.Database;
using ManagementSystem.Shared.Helpers;
using ManagementSystem.Web.Helpers;
using ManagementSystem.Web.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Security.Claims;
using System.Text;




var builder = WebApplication.CreateBuilder(args);

//Get information from Configuration File
//* Get the values from the appsettings file and map them with the class properties
var _settings = builder.Configuration
                .GetSection(nameof(MySQLSettings))
                .Get<MySQLSettings>() ??
                 throw new InvalidOperationException("Connection string not found.");

var _config = builder.Configuration;


//Add Application DI
builder.Services.AddApplication();

//Add Inventory Database DI
builder.Services
    .AddInventoryData(_config)
    .AddDatabase(_settings.ConnectionString);

builder.Services.AddAuthentication(opt =>
{
    // Set default authentication schemes for various scenarios
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Use cookie as the default
})
 .AddCookie(opt =>
{
    // Cookie-based authentication configuration
    opt.Cookie.Name = AppConstants.XAccessToken;   // Cookie name for authentication token
    opt.LoginPath = "/Users/Login";             // Path to the login page
    opt.AccessDeniedPath = "/Users/AccessDenied";
    opt.Cookie.SameSite = SameSiteMode.Strict; // Prevents CSRF attacks by ensuring cookies are not sent in cross-origin requests.
    opt.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensures cookies are only sent over HTTPS.
    opt.Cookie.HttpOnly = true; // Prevents client-side JavaScript from accessing cookies, reducing the risk of XSS attacks.
    opt.ExpireTimeSpan = TimeSpan.FromMinutes(Convert.ToInt32((_config["JwtSection:ExpiryMinutes"]))); // Sets a shorter token expiration time to reduce the window of opportunity for attacks if a token is compromised.
    opt.SlidingExpiration = true; // Extends the expiration time with each request, providing a better user experience while maintaining security.
    opt.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            // Log redirection to login
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Redirecting to login from: {Path}", context.Request.Path);

            // redirect to the login page
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        },
    };
})
.AddJwtBearer(opt =>
{
    // JWT bearer authentication configuration
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        
        ValidateIssuerSigningKey = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(60),       // Allow 60 minutes for clock skew
        ValidIssuer = _config["JwtSection:Issuer"]!,
        ValidAudience = _config["JwtSection:Audience"]!,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSection:Key"]!))
    };
    // Retrieve token from cookie for JWT bearer authentication
    opt.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            // Log the authentication failure
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(context.Exception, "Authentication failed.");

            // Redirect to login on authentication failure
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Redirect("/Users/Login");
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            // Log the challenge event
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("JWT challenge triggered. Redirecting to login.");

            // Redirect to login on 401 challenge
            if (context.Response.StatusCode != StatusCodes.Status401Unauthorized)
            {
                logger.LogInformation("Handling status code response. Redirecting to login.");
                context.HandleResponse(); // Suppress the default behavior
                context.Response.Redirect("/Users/Login");
            }
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Message received for JWT bearer authentication.");

            context.Token = context.Request.Cookies[AppConstants.XAccessToken];
            return Task.CompletedTask;
        }
    };
});

//add Authorisation and your policies
builder.Services.AddAuthorization(opt =>
{
    //Policy for Admin only
    opt.AddPolicy(AuthenticationConstants.AdminPolicyName,
        p => p.RequireClaim(claimType: ClaimTypes.Role, AuthenticationConstants.AdminClaimName));

    //Policy for User only
    opt.AddPolicy(AuthenticationConstants.UserPolicyName,
      p => p.RequireClaim(claimType: ClaimTypes.Role, AuthenticationConstants.UserClaimName));

    //Policy for User and Admin
    opt.AddPolicy(AuthenticationConstants.EveryUserPolicyName,
        p => p.RequireAssertion(c => c.User.HasClaim(ClaimTypes.Role, AuthenticationConstants.AdminClaimName) ||
                                c.User.HasClaim(ClaimTypes.Role, AuthenticationConstants.UserClaimName)));
});


// Add services to the container.
// Register MVC services with a global authorization filter
builder.Services.AddControllersWithViews(opt =>
{
    // This filter will require all controllers and actions to be authenticated by default
    //Enforces that all actions within the application require the user to be authenticated.
    // Add a global authorization filter
    var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
});

var app = builder.Build();

// Configure the localization options
var supportedCultures = new[] { new CultureInfo("en-zA") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-ZA"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures,
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

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Users}/{action=Login}/{id?}");

app.Run();
