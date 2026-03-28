using System.Security.Claims;
using EduTests.ApiObjects;
using EduTests.Commands.CommentCommands;
using EduTests.Commands.UserCommands;
using EduTests.Database.Entities;
using EduTests.Database.Enums;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class UsersController(
    IUserRepository userRepository,
    IBannedUserRepository bannedUserRepository,
    ICommentRepository commentRepository,
    IEntityToDtoService entityToDtoService) : ControllerBase
{
    /// <summary>
    /// Register in the system
    /// </summary>
    /// <param name="command">The <see cref="RegistrationCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkResult"/> or <see cref="BadRequestResult"/></returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
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
            try
            {
                await userRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {
                return BadRequest("User with that login already exists");
            }

            var apiUser = entityToDtoService.UserEntityToDto(user);
            
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

        var apiUser = entityToDtoService.UserEntityToDto(user);
        
        return Ok(apiUser);
    }
    
    /// <summary>
    /// Change the <see cref="ApiUser"/>'s name
    /// </summary>
    /// <param name="id">The <see cref="ApiUser"/> ID</param>
    /// <param name="command">The <see cref="ChangeUsernameCommand"/></param>
    /// <param name="context">The <see cref="HttpContext"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkResult"/> with the new <see cref="ApiUser"/> object
    /// (or <see cref="UnauthorizedResult"/> in case of an error)</returns>
    [HttpPatch("{id}/username")]
    [Authorize(Roles = "User, Moderator, Administrator")]
    public async Task<IActionResult> ChangeUsernameAsync(int id, [FromBody] ChangeUsernameCommand command, 
        CancellationToken cancellationToken = default)
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

        if (User.Identity is ClaimsIdentity claimsIdentity)
        {
            var givenNameClaim = claimsIdentity.FindFirst(ClaimTypes.GivenName);
            if (claimsIdentity.TryRemoveClaim(givenNameClaim))
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.GivenName, command.Username));
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal);
            }
        }

        var apiUser = entityToDtoService.UserEntityToDto(userToUpdate);
        
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
        
        var apiUser = entityToDtoService.UserEntityToDto(userToUpdate);
        
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
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }

    /// <summary>
    /// Get a specific <see cref="ApiComment"/> from this <see cref="ApiUser"/>'s profile
    /// </summary>
    /// <param name="id"><see cref="ApiUser"/> ID</param>
    /// <param name="commentId"><see cref="ApiComment"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="ApiComment"/> object</returns>
    [HttpGet("{id}/profilecomments/{commentId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProfileCommentAsync(int id, int commentId, 
        CancellationToken cancellationToken = default)
    {
        var comment = await commentRepository.GetWithLoadedCommenter(commentId, cancellationToken);
        if (comment is null)
            return NotFound();

        var apiComment = entityToDtoService.CommentEntityToDto(comment);
        return Ok(apiComment);
    }

    /// <summary>
    /// Get <see cref="ApiComment"/>s under <see cref="ApiUser"/>'s profile
    /// </summary>
    /// <param name="id"><see cref="ApiUser"/> ID</param>
    /// <param name="page">Page number</param>
    /// <param name="amountPerPage">Amount of <see cref="ApiReport"/>s per page</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>List of <see cref="ApiComment"/>s</returns>
    [HttpGet("{id}/profilecomments")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProfileCommentsAsync(int id, [FromQuery] int page,
        [FromQuery] int amountPerPage, CancellationToken cancellationToken = default)
    {
        if (page < 1 || amountPerPage < 1)
            return BadRequest("Invalid pagination parameters");

        var query = commentRepository.GetProfileComments(id);
        
        var commentCount = await query.CountAsync(cancellationToken);
        var pages = Math.Ceiling((double)commentCount / amountPerPage);

        var comments = await query
            .Skip((page - 1) * amountPerPage)
            .Take(amountPerPage)
            .ToListAsync(cancellationToken);

        var apiComments = comments.Select(entityToDtoService.CommentEntityToDto).ToList();

        return Ok(new {comments = apiComments, pages});
    }
    
    /// <summary>
    /// Create a <see cref="ApiComment"/> under <see cref="ApiUser"/>'s profile
    /// </summary>
    /// <param name="id"><see cref="ApiUser"/> ID</param>
    /// <param name="command">The <see cref="CreateCommentCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>Created <see cref="ApiComment"/></returns>
    [HttpPost("{id}/profilecomments")]
    [Authorize]
    public async Task<IActionResult> CreateProfileCommentAsync(int id, [FromBody] CreateCommentCommand command, 
        CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();
        
        var userIdInt = int.Parse(userId);
        
        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
            return NotFound();

        var comment = new Comment
        {
            CommenterId = userIdInt,
            UserProfileId = id,
            Content = command.Content
        };
        
        commentRepository.Create(comment);
        await commentRepository.SaveChangesAsync(cancellationToken);
        
        comment = await commentRepository.GetWithLoadedCommenter(comment.Id, cancellationToken);

        var apiComment = entityToDtoService.CommentEntityToDto(comment);
        
        return CreatedAtAction("GetProfileComment",  new { id = user.Id, commentId = comment.Id }, apiComment);
    }

    /// <summary>
    /// Delete a <see cref="ApiComment"/> under <see cref="ApiUser"/>'s profile
    /// </summary>
    /// <param name="id"><see cref="ApiUser"/> ID</param>
    /// <param name="commentId">The <see cref="ApiComment"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkResult"/></returns>
    [HttpDelete("{id}/profilecomments/{commentId}")]
    [Authorize]
    public async Task<IActionResult> DeleteProfileCommentAsync(int id, int commentId,
        CancellationToken cancellationToken = default)
    {
        var comment = await commentRepository.GetByIdAsync(commentId, cancellationToken);
        if (comment is null)
            return NotFound();
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();
        var userIdInt = int.Parse(userId);
        
        var role = User.FindFirstValue(ClaimTypes.Role);
        
        if (comment.CommenterId != userIdInt && role != "Moderator" && role != "Administrator")
            return Forbid();
            
        commentRepository.Delete(comment);
        await commentRepository.SaveChangesAsync(cancellationToken);
        
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

        var apiUser = entityToDtoService.UserEntityToDto(user);
        
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

        var apiBan = entityToDtoService.BanEntityToDto(ban);
        
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

        var apiBan = entityToDtoService.BanEntityToDto(ban);
        
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