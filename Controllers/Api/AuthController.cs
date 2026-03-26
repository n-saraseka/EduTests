using System.Security.Claims;
using EduTests.ApiObjects;
using EduTests.Commands.AuthCommands;
using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTests.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserRepository repository) : ControllerBase
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
        
        var apiUser = UserEntityToDto(user);
        return Ok(apiUser);
    }
    
    /// <summary>
    /// Map <see cref="User"/> entity to <see cref="ApiUser"/> DTO
    /// </summary>
    /// <param name="entity">The <see cref="User"/> entity</param>
    /// <returns>The <see cref="ApiUser"/> DTO</returns>
    private ApiUser UserEntityToDto(User entity)
    {
        var apiUser = new ApiUser
        {
            Id = entity.Id,
            Username = entity.Username,
            AvatarUrl = entity.AvatarUrl,
            Description = entity.Description,
            RegistrationDate = entity.RegistrationDate,
            Group = entity.Group
        };
        
        return apiUser;
    }
}