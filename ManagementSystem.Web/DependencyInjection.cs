using ManagementSystem.Application;
using ManagementSystem.Database;
using ManagementSystem.Shared.Helpers;
using ManagementSystem.Web.Helpers;
using ManagementSystem.Web.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Security.Claims;
using System.Text;

namespace ManagementSystem.Web
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationSevices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            var _config = builder.Configuration;
            //Get information from Configuration File
            //* Get the values from the appsettings file and map them with the class properties
            var _settings = _config.GetSection(nameof(MySQLSettings))
                            .Get<MySQLSettings>() ??
                             throw new InvalidOperationException("Connection string not found.");

            // Add application services
            services.AddApplication();

            // Add inventory database services
            services
                .AddInventoryData(_config)
                .AddDatabase(_settings.ConnectionString);

            // Add MVC services with global authorization filter
            services.AddControllersWithViews(opt =>
            {
                // This filter will require all controllers and actions to be authenticated by default
                //Enforces that all actions within the application require the user to be authenticated.
                // Add a global authorization filter
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            });

            // Ensure that all communications are done over HTTPS: <= For Production
            builder.Services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 443; // Ensure this is set to the correct HTTPS port
            });

            //Ensure your cookies are configured securely: <= For Production
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                options.HttpOnly = HttpOnlyPolicy.Always;
                options.Secure = CookieSecurePolicy.Always;
            });

            // Add authentication services
            services.AddAuthentication(opt =>
            {
                // Set default authentication schemes for various scenarios
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; ;
            })
             .AddCookie(opt =>
             {
                 // Cookie-based authentication configuration
                 opt.Cookie.Name = AppConstants.XAccessToken;   // Cookie name for authentication token
                 opt.LoginPath = "/Users/Login";             // Path to the login page
                 opt.AccessDeniedPath = "/Home/Error";
                 opt.Cookie.SameSite = SameSiteMode.Strict; // Prevents CSRF attacks by ensuring cookies are not sent in cross-origin requests.
                 opt.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensures cookies are only sent over HTTPS.
                 opt.Cookie.HttpOnly = true; // Prevents client-side JavaScript from accessing cookies, reducing the risk of XSS attacks: <= For Production if not make it false
                 opt.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Ensures the cookie expires after 30 minutes
                 opt.Cookie.MaxAge = TimeSpan.FromMinutes(30); // Ensures the cookie is considered valid for 30 minutes
                 opt.SlidingExpiration = true; // Extends the expiration time with each request, providing a better user experience while maintaining security.
                 
                 opt.Events = new CookieAuthenticationEvents
                 {
                     OnRedirectToLogin = context =>
                     {
                         // Log redirection to login
                         var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                         logger.LogInformation("Redirecting to login from: {Path}", context.Request.Path);

                         // Redirect to the login page
                         context.Response.Redirect(context.RedirectUri);
                         return Task.CompletedTask;
                     },
                 };
             })
            .AddJwtBearer(opt =>
            {

                opt.RequireHttpsMetadata = true; // Ensure HTTPS is required
                opt.SaveToken = true; // Saves the token in the HttpContext

                // JWT bearer authentication configuration
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,    // Validate token lifetime
                    ClockSkew = TimeSpan.Zero, // No clock skew allowed
                    ValidIssuer = _config["JwtSection:Issuer"]!,
                    ValidAudience = _config["JwtSection:Audience"]!,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSection:Key"]!)),
                   
                    // Set the token lifetime to 30 minutes
                    LifetimeValidator = (before, expires, token, param) =>
                    {
                        // Ensure token expiration is greater than current time
                        return expires != null && expires > DateTime.UtcNow;
                    }
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

                        // Redirect to login on 401 challenge if user is unauthenticated
                        if (!context.HttpContext.User.Identity!.IsAuthenticated)
                        {
                            logger.LogInformation("Handling unauthenticated user. Redirecting to login.");
                            context.HandleResponse(); // Suppress the default behavior
                            context.Response.Redirect("/Users/Login");
                        }
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogInformation("Message received for JWT bearer authentication.");

                        // Retrieve token from cookie for JWT bearer authentication
                        var token = context.Request.Cookies[AppConstants.XAccessToken];
                        if (string.IsNullOrEmpty(token))
                        {
                            logger.LogWarning("No token found in cookies.");
                        }
                        else
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // Add authorization policies
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy(AuthenticationConstants.LoggedInPolicyName, policy =>
                        policy.RequireAuthenticatedUser());

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

            return services;
        }

        public static WebApplication BuildAndConfigure(this WebApplicationBuilder builder)
        {
            var app = builder.Build();

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

                // Ensure all communications are done over HTTPS
                //app.UseHttpsRedirection();
            }


            // Enforce HTTPS
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Use authentication and authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Add custom exception handling middleware
            app.UseMiddleware<CheckAuthenticationMiddleware>();

            // Map controller routes
            app.MapControllerRoute(
                name: "login",
                pattern: "Users/Login",
                defaults: new { controller = "Users", action = "Login" });

            app.MapControllerRoute(
                name: "registration",
                pattern: "Users/Registration",
                defaults: new { controller = "Users", action = "Registration" });

              // Map default controller route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .RequireAuthorization(AuthenticationConstants.LoggedInPolicyName);

            return app;
        }
    }
}
