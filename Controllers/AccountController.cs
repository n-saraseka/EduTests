using EduTests.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTests.Controllers;

public class AccountController() : Controller
{
    [HttpGet]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        return View(model);
    }
    
    [HttpGet]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        return View(model);
    }

    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}