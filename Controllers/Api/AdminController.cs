using System.Security.Claims;
using EduTests.ApiObjects;
using EduTests.Commands;
using EduTests.Database.Entities;
using EduTests.Database.Enums;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTests.Controllers.Api;


[ApiController]
[Route("api/[controller]")]
public class AdminController(IUserRepository userRepository,
    IBannedUserRepository bansRepository,
    IReportsRepository reportsRepository) : ControllerBase
{
    /// <summary>
    /// Promote a <see cref="ApiUser"/> to <see cref="UserGroup.Moderator"/>
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("promote-to-moderator")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> PromoteToModeratorAsync([FromBody] PromoteToModeratorCommand command,
        CancellationToken cancellationToken = default)
    {
        var userToPromote = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (userToPromote is null)
            return BadRequest("User does not exist");

        if (userToPromote.Group != UserGroup.User)
            return BadRequest("User had already been promoted");

        userToPromote.Group = UserGroup.Moderator;
        await userRepository.SaveChangesAsync(cancellationToken);
        
        var apiUser = new ApiUser
        {
            Id = userToPromote.Id,
            Username = userToPromote.Username,
            AvatarUrl = userToPromote.AvatarUrl,
            Description = userToPromote.Description,
            RegistrationDate = userToPromote.RegistrationDate,
            Group = userToPromote.Group
        };
        
        return Ok(apiUser);
    }


    [HttpPost("ban-user")]
    [Authorize(Roles = "Moderator, Administrator")]
    public async Task<IActionResult> BanUserAsync([FromBody] BanUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var userToBan = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (userToBan is null)
            return BadRequest("User does not exist");
        
        var activeBans = await bansRepository.GetUsersActiveBanAsync(command.UserId, cancellationToken);
        if (activeBans != null)
            return BadRequest("User has an active ban");
        
        var bannedUser = new BannedUser
        {
            UserBannedId = command.UserId,
            BannedById = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
            BanReason = command.Reason,
            DateBanned = DateTime.UtcNow,
            DateUnbanned = command.UnbanDate
        };

        bansRepository.Create(bannedUser);
        await bansRepository.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpPost("change-report-status")]
    [Authorize(Roles = "Moderator, Administrator")]
    public async Task<IActionResult> ChangeReportStatusAsync([FromBody] ChangeReportStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var report = await reportsRepository.GetByIdAsync(command.ReportId, cancellationToken);
        if (report == null)
            return BadRequest("Report does not exist");
        if (report.ReportStatus == command.Status)
            return BadRequest("Report status is the same as requested");
        report.ReportStatus = command.Status;
        await reportsRepository.SaveChangesAsync(cancellationToken);

        var updatedReport = new ApiReport
        {
            Id = report.Id,
            UserId = report.UserId,
            CommentId = report.CommentId,
            TestId = report.TestId,
            ReportText = report.Text,
            DateReported = report.DateTime,
            ReportStatus = report.ReportStatus
        };
        return Ok(updatedReport);
    }

    [HttpPost("lift-ban")]
    [Authorize(Roles = "Moderator, Administrator")]
    public async Task<IActionResult> LiftBanAsync([FromBody] LiftBanCommand command,
        CancellationToken cancellationToken = default)
    {
        var ban = await bansRepository.GetByIdAsync(command.BanId, cancellationToken);
        if (ban is null)
            return BadRequest("Ban does not exist");
        
        bansRepository.Delete(ban);
        await bansRepository.SaveChangesAsync(cancellationToken);
        return Ok();
    }
}