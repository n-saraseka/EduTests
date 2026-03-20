using System.Security.Claims;
using EduTests.Commands;
using EduTests.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTests.Controllers.Api;

[ApiController]
public class AuthController(IUserAuthenticationService service) : ControllerBase
{
    /// <summary>
    /// Log into the system
    /// </summary>
    /// <param name="command">The <see cref="LoginCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkResult"/> or <see cref="UnauthorizedResult"/> in case of an error</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await service.ValidateUserAsync(command.Login, command.Password, cancellationToken);
        if (user == null)
            return Unauthorized();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, user.Group.ToString()),
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        
        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));
        return Ok();
    }
    
    /// <summary>
    /// Register in the system
    /// </summary>
    /// <param name="command">The <see cref="RegistrationCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkResult"/> or <see cref="BadRequestResult"/> / Status code 500 in case of an error</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync([FromBody] RegistrationCommand command, CancellationToken cancellationToken)
    {

        try
        {
            var user = await service.RegisterAsync(command.Login, command.Password, command.Username, cancellationToken);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, user.Group.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest("User with that login already exists");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }
}