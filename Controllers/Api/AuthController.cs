using System.Security.Claims;
using EduTests.Commands.AuthCommands;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTests.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserRepository repository,
    IEntityToDtoService entityToDtoService) : ControllerBase
{
    /// <summary>
    /// Log into the system
    /// </summary>
    /// <param name="command">The <see cref="LoginCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkResult"/> or <see cref="UnauthorizedResult"/> in case of an error</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginAsync([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await repository.GetByLoginAsync(command.Login, cancellationToken);
        if (user == null || !BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash))
            return BadRequest("Username or password is incorrect");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, user.Group.ToString()),
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.GivenName, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        
        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));
        
        var apiUser = entityToDtoService.UserEntityToDto(user);
        return Ok(apiUser);
    }
}