using System.Security.Claims;
using EduTests.ApiObjects;
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
    public async Task<IActionResult> ChangeUsernameAsync(string newUsername, CancellationToken cancellationToken = default)
    {
        var login = User.FindFirstValue(ClaimTypes.Name);
        if (login is null)
            return Unauthorized();

        var user = await repository.GetByLoginAsync(login, cancellationToken);
        if (user is null)
            return Unauthorized();

        user.Username = newUsername;
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
    public async Task<IActionResult> ChangeDescriptionAsync(string newDescription, CancellationToken cancellationToken = default)
    {
        var login = User.FindFirstValue(ClaimTypes.Name);
        if (login is null)
            return Unauthorized();

        var user = await repository.GetByLoginAsync(login, cancellationToken);
        if (user is null)
            return Unauthorized();

        user.Description = newDescription;
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
    
    [HttpPut("change-password")]
    [Authorize(Roles = "User, Moderator, Administrator")]
    public async Task<IActionResult> ChangePasswordAsync(string newPassword, CancellationToken cancellationToken = default)
    {
        var login = User.FindFirstValue(ClaimTypes.Name);
        if (login is null)
            return Unauthorized();

        var user = await repository.GetByLoginAsync(login, cancellationToken);
        if (user is null)
            return Unauthorized();

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
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
}