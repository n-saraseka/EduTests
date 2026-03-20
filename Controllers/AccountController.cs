using EduTests.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTests.Controllers;

public class AccountController() : Controller
{
    /// <summary>
    /// Get the Login page
    /// </summary>
    /// <param name="model">The <see cref="LoginViewModel"/></param>
    /// <returns>The <see cref="ViewResult"/> with the <see cref="LoginViewModel"/></returns>
    [HttpGet]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        return View(model);
    }
    
    /// <summary>
    /// Get to the Register page
    /// </summary>
    /// <param name="model">The <see cref="RegisterViewModel"/></param>
    /// <returns>The <see cref="ViewResult"/> with the <see cref="RegisterViewModel"/></returns>
    [HttpGet]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        return View(model);
    }

    /// <summary>
    /// Log out of the system
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>A <see cref="RedirectToActionResult"/> (redirects to the home page)</returns>
    [HttpGet]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}