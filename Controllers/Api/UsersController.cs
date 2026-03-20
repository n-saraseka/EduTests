using System.Security.Claims;
using EduTests.ApiObjects;
using EduTests.Commands;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTests.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserRepository repository) : ControllerBase
{
    /// <summary>
    /// Change the <see cref="ApiUser"/>'s name
    /// </summary>
    /// <param name="command">The <see cref="ChangeUsernameCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkResult"/> with the new <see cref="ApiUser"/> object
    /// (or <see cref="UnauthorizedResult"/> in case of an error)</returns>
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
    
    /// <summary>
    /// Change the <see cref="ApiUser"/>'s profile description
    /// </summary>
    /// <param name="command">The <see cref="ChangeDescriptionCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkResult"/> with the new <see cref="ApiUser"/> object
    /// (or <see cref="UnauthorizedResult"/> in case of an error)</returns>
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

    /// <summary>
    /// Change the <see cref="ApiUser"/>'s login
    /// </summary>
    /// <param name="command">The <see cref="ChangeLoginCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkResult"/> (or <see cref="UnauthorizedResult"/> / <see cref="BadRequestResult"/> in case of an error)</returns>
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
    
    /// <summary>
    /// Change the <see cref="ApiUser"/>'s password
    /// </summary>
    /// <param name="command">The <see cref="ChangePasswordCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkResult"/> (or <see cref="UnauthorizedResult"/> / <see cref="BadRequestResult"/> in case of an error)</returns>
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

    /// <summary>
    /// Delete the <see cref="ApiUser"/>
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkResult"/> (or <see cref="UnauthorizedResult"/> in case of an error)</returns>
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