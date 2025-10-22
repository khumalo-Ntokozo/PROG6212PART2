using Microsoft.AspNetCore.Mvc;
using ProgrammingPOE.Models;
using ProgrammingPOE.Services;
using ProgrammingPOE.ViewModels;

namespace ProgrammingPOE.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;
        private readonly DataService _dataService;

        public AccountController(AuthService authService, DataService dataService)
        {
            _authService = authService;
            _dataService = dataService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_authService.Login(model.Email, model.Password))
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Role = model.Role,
                    Password = model.Password
                };

                _dataService.AddUser(user);
                _authService.Login(model.Email, model.Password);

                TempData["Message"] = "Registration successful!";
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            _authService.Logout();
            return RedirectToAction("Index", "Home");
        }
    }
}