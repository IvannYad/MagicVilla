using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        public async Task<IActionResult> LoginPOST(LoginRequestDTO login)
        {
            APIResponse response = await _authService.LoginAsync<APIResponse>(login);
            if (response is not null && response.IsSuccess)
            {
                // If login is successfull.
                LoginResponseDTO loginResponse = JsonConvert
                    .DeserializeObject<LoginResponseDTO>(Convert.ToString(response.Result)!)!;
                HttpContext.Session.SetString(SD.SessionToken, loginResponse.Token);
                TempData["success"] = "Login successfully";
                return RedirectToAction(nameof(Index), "Home");
            }

            ModelState.AddModelError("CustomError", response.ErrorMessages.FirstOrDefault());
            TempData["success"] = "Login failed";
            return View(nameof(Login), login);
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
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString(SD.SessionToken, string.Empty);
            return RedirectToAction(nameof(Index), "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
