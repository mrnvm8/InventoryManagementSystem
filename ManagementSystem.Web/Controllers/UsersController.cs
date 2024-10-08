﻿using ManagementSystem.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.Web.Controllers
{

    public class UsersController(IAuthService _authService) : BaseController
    {
        [Authorize(AuthenticationConstants.AdminPolicyName)]
        public async Task<IActionResult> Index(CancellationToken token = default)
        {
            var result = await _authService.GetUsers(token);
            if (result.IsSuccess)
            {
                ViewBag.Error = result.Message;
            }
            return View(result.Data);
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (HttpContext.User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // Post: Users/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] UserLoginRequest request, CancellationToken token = default)
        {
            //Login into the System
            var response = await _authService.Login(request, token);

            if (!response.IsSuccess)
            {
                //Get the error
                ViewBag.AuthError = response.Message;
                //Stay on the same login page
                return View();
            }

            //Save the token to cookies
            SaveTokenCookies(response.Data.Token);

            return RedirectToAction("Index", "Home");
        }


        // GET: UsersController1/Create
        [AllowAnonymous]
        public ActionResult UserRegistration()
        {
            if (HttpContext.User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UserRegistration([FromForm] UserRequest request, CancellationToken token = default)
        {
            //register user
            var response = await _authService.Register(request, token);
            if (!response.IsSuccess)
            {
                ViewBag.AuthError = response.Message.Split("/").Reverse();
                //Stay on the rgistration page
                //with the error message.
                return View();
            }

            SaveTokenCookies(response.Data.Token);

            return RedirectToAction("Index", "Home");
        }

        [Authorize(AuthenticationConstants.AdminPolicyName)]
        public IActionResult PasswordChange(Guid id, CancellationToken token = default)
        {
            if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
            {
                ModelState.AddModelError("", new ControllersErrors().InvalidID("User"));
                return View("Error");
            }

            return View();
        }

        // POST: Users/passwordChange
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationConstants.AdminPolicyName)]
        public async Task<IActionResult> PasswordChange(Guid id, [FromForm] UserRequest request, CancellationToken token = default)
        {
            if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
            {
                ModelState.AddModelError("", new ControllersErrors().InvalidID("User"));
                return View("Error");
            }

            var result = await _authService.ChangeUserPassword(id, request.Password);
            if (!result.IsSuccess)
            {
                //convering the string to array of error message
                ViewBag.Error = result.Message!.Split(",").Reverse();
                return View();
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(AuthenticationConstants.AdminPolicyName)]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
            {
                ModelState.AddModelError("", new ControllersErrors().InvalidID("User"));
                return View();
            }

            var result = await _authService.GetUserById(id);
            if (!result.IsSuccess)
            {
                ViewBag.Error = result.Message;
                return View();
            }
            return View(result.Data);
        }

        // POST: DeviceTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationConstants.AdminPolicyName)]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (!Guid.TryParse(id.ToString(), out _) || id == Guid.Empty)
            {
                ModelState.AddModelError("", new ControllersErrors().InvalidID("User"));
                return View();
            }

            var result = await _authService.RemoveUserAsync(id);
            if (!result.IsSuccess)
            {
                ViewBag.Error = result.Message;
                return View();
            }
            return RedirectToAction(nameof(Index));
        }
        private void SaveTokenCookies(string Token)
        {
            Response.Cookies.Append(
                AppConstants.XAccessToken,
                Token,
                new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    Secure = true,
                    IsEssential = true,
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict
                });
        }
    }
}
