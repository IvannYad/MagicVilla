using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var login = new LoginRequestDTO();
            return View(login);
        }

        [HttpPost]
        [ActionName("Login")]
        [ValidateAntiForgeryToken]
        public IActionResult LoginPOST(LoginRequestDTO login)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            var register = new RegistrationRequestDTO();
            return View(register);
        }

        [HttpPost]
        [ActionName("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPOST(RegistrationRequestDTO registration)
        {
            if (!string.Equals(registration.Password, registration.PasswordConfirm, StringComparison.CurrentCulture))
                ModelState.AddModelError(nameof(registration.PasswordConfirm), "Passwords must match");

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Registration failed";
                var register = new RegistrationRequestDTO();
                return View(nameof(Register), register);
            }

            APIResponse response =  await _authService.RegisterAsync<APIResponse>(registration);
            if (response is not null && response.IsSuccess)
            {
                TempData["success"] = "Registrated successfully";
                return RedirectToAction(nameof(Login));
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
