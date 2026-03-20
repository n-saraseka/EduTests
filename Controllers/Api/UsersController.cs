using System.Security.Claims;
using EduTests.ApiObjects;
using EduTests.Commands;
using EduTests.Database.Enums;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTests.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserRepository repository) : ControllerBase
{
    [HttpPut("change-username")]
    [Authorize(Roles = "User, Moderator, Administrator")]
    public async Task<IActionResult> ChangeUsernameAsync([FromBody] ChangeUsernameCommand command, CancellationToken cancellationToken = default)
    {
        var login = User.FindFirstValue(ClaimTypes.Name);
        if (login is null)
            return Unauthorized();

        var user = await repository.GetByLoginAsync(login, cancellationToken);
        if (user is null || user.Login != login)
            return Unauthorized();

        user.Username = command.Username;
        repository.Update(user);
        await repository.SaveChangesAsync(cancellationToken);

        var apiUser = new ApiUser
        {
            Id = user.Id,
            Username = user.Username,
            AvatarUrl = user.AvatarUrl,
            Description = user.Description,
            RegistrationDate = user.RegistrationDate,
            Group = user.Group
        };
        
        return Ok(apiUser);
    }
    
    [HttpPut("change-description")]
    [Authorize(Roles = "User, Moderator, Administrator")]
    public async Task<IActionResult> ChangeDescriptionAsync([FromBody] ChangeDescriptionCommand command, CancellationToken cancellationToken = default)
    {
        var login = User.FindFirstValue(ClaimTypes.Name);
        if (login is null)
            return Unauthorized();

        var user = await repository.GetByLoginAsync(login, cancellationToken);
        if (user is null || user.Login != login)
            return Unauthorized();

        user.Description = command.Description;
        repository.Update(user);
        await repository.SaveChangesAsync(cancellationToken);
        
        var apiUser = new ApiUser
        {
            Id = user.Id,
            Username = user.Username,
            AvatarUrl = user.AvatarUrl,
            Description = user.Description,
            RegistrationDate = user.RegistrationDate,
            Group = user.Group
        };
        
        return Ok(apiUser);
    }

    public async Task<IActionResult> ChangeLoginAsync([FromBody] ChangeLoginCommand command,
        CancellationToken cancellationToken = default)
    {
        var login = User.FindFirstValue(ClaimTypes.Name);
        if (login is null)
            return Unauthorized();
        
        var user = await repository.GetByLoginAsync(login, cancellationToken);
        if (user is null || user.Login != login)
            return Unauthorized();
        
        if (!BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash)) 
            return BadRequest("Passwords do not match");
        
        user.Login = command.Login;
        repository.Update(user);
        await repository.SaveChangesAsync(cancellationToken);
        
        return Ok();
    }
    
    [HttpPut("change-password")]
    [Authorize(Roles = "User, Moderator, Administrator")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordCommand command, 
        CancellationToken cancellationToken = default)
    {
        var login = User.FindFirstValue(ClaimTypes.Name);
        if (login is null)
            return Unauthorized();

        var user = await repository.GetByLoginAsync(login, cancellationToken);
        if (user is null || user.Login != login)
            return Unauthorized();
        
        if (!BCrypt.Net.BCrypt.Verify(command.OldPassword, user.PasswordHash))
            return BadRequest("Passwords do not match");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.NewPassword);
        repository.Update(user);
        await repository.SaveChangesAsync(cancellationToken);
        
        return Ok();
    }

    [HttpGet("delete-user")]
    [Authorize(Roles = "User, Moderator, Administrator")]
    public async Task<IActionResult> DeleteUserAsync(CancellationToken cancellationToken = default)
    {
        var login = User.FindFirstValue(ClaimTypes.Name);
        if (login is null)
            return Unauthorized();

        var user = await repository.GetByLoginAsync(login, cancellationToken);
        if (user is null || user.Login != login)
            return Unauthorized();
        
        repository.Delete(user);
        await repository.SaveChangesAsync(cancellationToken);
        return Ok();
    }
}