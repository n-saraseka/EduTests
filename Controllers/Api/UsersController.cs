using System.Security.Claims;
using EduTests.ApiObjects;
using EduTests.Commands.UserCommands;
using EduTests.Database.Entities;
using EduTests.Database.Enums;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTests.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class UsersController(
    IUserRepository userRepository,
    IBannedUserRepository bannedUserRepository) : ControllerBase
{
    /// <summary>
    /// Register in the system
    /// </summary>
    /// <param name="command">The <see cref="RegistrationCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkResult"/> or <see cref="BadRequestResult"/></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync([FromBody] RegistrationCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var user = new User
            {
                Username = command.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                Login = command.Login,
                Group = UserGroup.User,
                RegistrationDate = DateTime.UtcNow
            };
            
            userRepository.Create(user);
            await userRepository.SaveChangesAsync(cancellationToken);

            var apiUser = new ApiUser
            {
                Id = user.Id,
                Username = user.Username,
                Group = user.Group,
                RegistrationDate = user.RegistrationDate
            };
            
            return CreatedAtAction("GetUser",
                new { id = user.Id }, apiUser);
        }
        catch (InvalidOperationException)
        {
            return BadRequest("User with that login already exists");
        }
    }

    /// <summary>
    /// Get an <see cref="ApiUser"/>
    /// </summary>
    /// <param name="id"><see cref="ApiUser"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>The respective <see cref="ApiUser"/> object</returns>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
            return NotFound();

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
    /// Change the <see cref="ApiUser"/>'s name
    /// </summary>
    /// <param name="id">The <see cref="ApiUser"/> ID</param>
    /// <param name="command">The <see cref="ChangeUsernameCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkResult"/> with the new <see cref="ApiUser"/> object
    /// (or <see cref="UnauthorizedResult"/> in case of an error)</returns>
    [HttpPatch("{id}/username")]
    [Authorize(Roles = "User, Moderator, Administrator")]
    public async Task<IActionResult> ChangeUsernameAsync(int id, [FromBody] ChangeUsernameCommand command, CancellationToken cancellationToken = default)
    {
        var login = User.FindFirstValue(ClaimTypes.Name);
        var role = User.FindFirstValue(ClaimTypes.Role);
        
        var userToUpdate = await userRepository.GetByIdAsync(id, cancellationToken);
        if (userToUpdate is null)
            return NotFound();
        if (userToUpdate.Login != login && role != "Administrator" && role != "Moderator")
            return Forbid();

        userToUpdate.Username = command.Username;
        userRepository.Update(userToUpdate);
        await userRepository.SaveChangesAsync(cancellationToken);

        var apiUser = new ApiUser
        {
            Id = userToUpdate.Id,
            Username = userToUpdate.Username,
            AvatarUrl = userToUpdate.AvatarUrl,
            Description = userToUpdate.Description,
            RegistrationDate = userToUpdate.RegistrationDate,
            Group = userToUpdate.Group
        };
        
        return Ok(apiUser);
    }
    
    /// <summary>
    /// Change the <see cref="ApiUser"/>'s profile description
    /// </summary>
    /// <param name="id">The <see cref="ApiUser"/> ID</param>
    /// <param name="command">The <see cref="ChangeDescriptionCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkResult"/> with the new <see cref="ApiUser"/> object
    /// (or <see cref="UnauthorizedResult"/> in case of an error)</returns>
    [HttpPatch("{id}/description")]
    [Authorize(Roles = "User, Moderator, Administrator")]
    public async Task<IActionResult> ChangeUserDescriptionAsync(int id, [FromBody] ChangeDescriptionCommand command, CancellationToken cancellationToken = default)
    {
        var login = User.FindFirstValue(ClaimTypes.Name);
        var role = User.FindFirstValue(ClaimTypes.Role);
        
        var userToUpdate = await userRepository.GetByIdAsync(id, cancellationToken);
        if (userToUpdate is null)
            return NotFound();
        if (userToUpdate.Login != login && role != "Administrator" && role != "Moderator")
            return Forbid();

        userToUpdate.Description = command.Description;
        userRepository.Update(userToUpdate);
        await userRepository.SaveChangesAsync(cancellationToken);
        
        var apiUser = new ApiUser
        {
            Id = userToUpdate.Id,
            Username = userToUpdate.Username,
            AvatarUrl = userToUpdate.AvatarUrl,
            Description = userToUpdate.Description,
            RegistrationDate = userToUpdate.RegistrationDate,
            Group = userToUpdate.Group
        };
        
        return Ok(apiUser);
    }

    /// <summary>
    /// Change the <see cref="ApiUser"/>'s login
    /// </summary>
    /// <param name="id">The <see cref="ApiUser"/> ID</param>
    /// <param name="command">The <see cref="ChangeLoginCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkResult"/> (or <see cref="UnauthorizedResult"/> / <see cref="BadRequestResult"/> in case of an error)</returns>
    [HttpPatch("{id}/login")]
    [Authorize(Roles = "User, Moderator, Administrator")]
    public async Task<IActionResult> ChangeLoginAsync(int id, [FromBody] ChangeLoginCommand command,
        CancellationToken cancellationToken = default)
    {
        var login = User.FindFirstValue(ClaimTypes.Name);
        
        var userToUpdate = await userRepository.GetByIdAsync(id, cancellationToken);
        if (userToUpdate is null)
            return NotFound();
        if (userToUpdate.Login != login)
            return Forbid();
        
        if (!BCrypt.Net.BCrypt.Verify(command.Password, userToUpdate.PasswordHash)) 
            return BadRequest("Passwords do not match");
        
        var existingUser = await userRepository.GetByLoginAsync(command.Login, cancellationToken);
        if (existingUser is not null)
            return Conflict("User with that login already exists");
        userToUpdate.Login = command.Login;
        userRepository.Update(userToUpdate);
        await userRepository.SaveChangesAsync(cancellationToken);
        return Ok();
    }
    
    /// <summary>
    /// Change the <see cref="ApiUser"/>'s password
    /// </summary>
    /// <param name="id">The <see cref="ApiUser"/> ID</param>
    /// <param name="command">The <see cref="ChangePasswordCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkResult"/> (or <see cref="UnauthorizedResult"/> / <see cref="BadRequestResult"/> in case of an error)</returns>
    [HttpPatch("{id}/password")]
    [Authorize(Roles = "User, Moderator, Administrator")]
    public async Task<IActionResult> ChangePasswordAsync(int id, [FromBody] ChangePasswordCommand command, 
        CancellationToken cancellationToken = default)
    {
        var login = User.FindFirstValue(ClaimTypes.Name);
        
        var userToUpdate = await userRepository.GetByIdAsync(id, cancellationToken);
        if (userToUpdate is null)
            return NotFound();
        if (userToUpdate.Login != login)
            return Forbid();
        
        if (!BCrypt.Net.BCrypt.Verify(command.OldPassword, userToUpdate.PasswordHash))
            return BadRequest("Passwords do not match");

        userToUpdate.PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.NewPassword);
        userRepository.Update(userToUpdate);
        await userRepository.SaveChangesAsync(cancellationToken);
        
        return Ok();
    }

    /// <summary>
    /// Delete the <see cref="ApiUser"/>
    /// </summary>
    /// <param name="id">The <see cref="ApiUser"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkResult"/> (or <see cref="UnauthorizedResult"/> in case of an error)</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "User, Moderator, Administrator")]
    public async Task<IActionResult> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        var login = User.FindFirstValue(ClaimTypes.Name);
        var role = User.FindFirstValue(ClaimTypes.Role);

        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
            return NotFound();
        if (user.Login != login && role != "Administrator")
            return Forbid();
        
        userRepository.Delete(user);
        await userRepository.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Promote a <see cref="ApiUser"/> to <see cref="UserGroup.Moderator"/>
    /// </summary>
    /// <param name="id"><see cref="ApiUser"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>An <see cref="ApiUser"/> object</returns>
    [HttpPatch("{id}/moderator")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> PromoteToModeratorAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
            return NotFound();
        
        if (user.Group == UserGroup.Moderator)
            return BadRequest("User is already a moderator");
        
        user.Group = UserGroup.Moderator;
        userRepository.Update(user);
        await userRepository.SaveChangesAsync(cancellationToken);

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
    /// Ban a <see cref="ApiUser"/>
    /// </summary>
    /// <param name="id"><see cref="ApiUser"/> ID</param>
    /// <param name="command">The <see cref="BanUserCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>A <see cref="ApiBan"/> object</returns>
    [HttpPost("{id}/bans")]
    [Authorize(Roles = "Moderator, Administrator")]
    public async Task<IActionResult> BanUserAsync(int id, [FromBody] BanUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
            return NotFound();

        var userBans = await bannedUserRepository.GetUsersActiveBanAsync(id, cancellationToken);
        if (userBans != null)
            return BadRequest("User has already been banned");
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Forbid();
        
        var userIdInt = int.Parse(userId);

        var ban = new BannedUser
        {
            BannedById = userIdInt,
            UserBannedId = id,
            BanReason = command.Reason,
            DateBanned = DateTime.UtcNow,
            DateUnbanned = DateTime.UtcNow,
        };
        
        bannedUserRepository.Create(ban);

        var apiBan = new ApiBan
        {
            Id = ban.Id,
            BannedUserId = id,
            BannedByUserId = ban.BannedById,
            BanReason = command.Reason,
            BanDate = ban.DateBanned,
            UnbanDate = ban.DateUnbanned
        };
        
        return CreatedAtAction("GetUserBan",  new { id = ban.Id }, apiBan);
    }

    /// <summary>
    /// Get a <see cref="ApiUser"/>'s <see cref="ApiBan"/>
    /// </summary>
    /// <param name="id"><see cref="ApiUser"/> ID</param>
    /// <param name="banId"><see cref="ApiBan"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>to observe</param>
    /// <returns>A <see cref="ApiBan"/> object</returns>
    [HttpGet("{id}/bans/{banId}")]
    [Authorize(Roles = "Moderator, Administrator")]
    public async Task<IActionResult> GetUserBanAsync(int id, int banId, CancellationToken cancellationToken = default)
    {
        var ban = await bannedUserRepository.GetByIdAsync(id, cancellationToken);
        if (ban is null)
            return NotFound();
        
        var apiBan = new ApiBan
        {
            Id = ban.Id,
            BannedUserId = id,
            BannedByUserId = ban.BannedById,
            BanReason = ban.BanReason,
            BanDate = ban.DateBanned,
            UnbanDate = ban.DateUnbanned
        };
        
        return Ok(apiBan);
    }

    /// <summary>
    /// Lift a <see cref="ApiUser"/>'s ban
    /// </summary>
    /// <param name="id"><see cref="ApiUser"/> ID</param>
    /// <param name="banId"><see cref="ApiBan"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkObjectResult"/></returns>
    [HttpDelete("{id}/bans/{banId}")]
    [Authorize(Roles = "Moderator, Administrator")]
    public async Task<IActionResult> LiftUserBanAsync(int id, int banId, CancellationToken cancellationToken = default)
    {
        var ban = await bannedUserRepository.GetByIdAsync(id, cancellationToken);
        if (ban is null)
            return NotFound();
        
        bannedUserRepository.Delete(ban);
        return Ok();
    }
}