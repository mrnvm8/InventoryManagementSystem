using ManagementSystem.Web.Helpers;
using ManagementSystem.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ManagementSystem.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(AuthenticationConstants.AdminPolicyName)]
        public IActionResult Settings()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Remove the authentication token from cookies
            Response.Cookies.Delete(AppConstants.XAccessToken);

            return RedirectToAction("Login", "Users");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        // Error action to handle and display error messages
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            var errorViewModel = new ErrorViewModel
            {
                Message = message ?? "An unexpected error occurred. Please try again later."
            };

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = message });
        }



    }
}
