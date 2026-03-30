using System.Security.Claims;
using EduTests.ApiObjects;
using EduTests.Commands.ReportCommands;
using EduTests.Database.Entities;
using EduTests.Database.Enums;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Controllers.Api;


[ApiController]
[Route("api/[controller]")]
public class ReportController(IReportsRepository reportsRepository,
    IUserRepository userRepository,
    IAnonymousUserRepository anonymousUserRepository,
    ICommentRepository commentRepository,
    ITestRepository testRepository,
    IEntityToDtoService entityToDtoService) : ControllerBase
{
    /// <summary>
    /// Create a <see cref="ApiReport"/>
    /// </summary>
    /// <param name="command">The <see cref="CreateReportCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="CreatedResult"/></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateReportAsync([FromBody] CreateReportCommand command,
        CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAuthenticated = User.Identity?.IsAuthenticated ?? false;

        int? authenticatedUserId = isAuthenticated ? int.Parse(userId) : null;
        Guid? anonymousUserId = !isAuthenticated ? Guid.Parse(userId) : null;

        if (!isAuthenticated)
        {
            var existingAnon = await anonymousUserRepository.GetByIdAsync((Guid)anonymousUserId, cancellationToken);
            if (existingAnon is null)
            {
                var anon = new AnonymousUser
                {
                    Id = (Guid)anonymousUserId
                };
                anonymousUserRepository.Create(anon);
                await anonymousUserRepository.SaveChangesAsync(cancellationToken);
            }
        }

        Report? existingReport = null;
        Report? report = null;
        
        switch (command.EntityType)
            {
                case EntityType.Test:
                    var test = await testRepository.GetByIdAsync(command.EntityId, cancellationToken);
                    if (test is null)
                        return NotFound();
                    
                    existingReport = await reportsRepository.
                        GetByTestAndReporterIdAsync(test.Id, authenticatedUserId, anonymousUserId, cancellationToken);
                    
                    report = new Report
                    {
                        TestId = test.Id,
                        Text = command.Text,
                        DateTime = DateTime.UtcNow,
                        ReportStatus = ReportStatus.Pending
                    };

                    break;
                case EntityType.User:
                    var user = await userRepository.GetByIdAsync(command.EntityId, cancellationToken);
                    if (user is null)
                        return NotFound();

                    existingReport = await reportsRepository.
                        GetByUserAndReporterIdAsync(user.Id, authenticatedUserId, anonymousUserId, cancellationToken);

                    if (user.Id == authenticatedUserId)
                        return BadRequest("Self reporting is not allowed");
                    
                    report = new Report
                    {
                        UserId = user.Id,
                        Text = command.Text,
                        DateTime = DateTime.UtcNow,
                        ReportStatus = ReportStatus.Pending
                    };
                    break;
                case EntityType.Comment:
                    var comment = await commentRepository.GetByIdAsync(command.EntityId, cancellationToken);
                    if (comment is null)
                        return NotFound();
                    
                    existingReport = await reportsRepository.
                        GetByCommentAndReporterIdAsync(comment.Id, authenticatedUserId, anonymousUserId, cancellationToken);
                    
                    report = new Report
                    {
                        CommentId = comment.Id,
                        Text = command.Text,
                        DateTime = DateTime.UtcNow,
                        ReportStatus = ReportStatus.Pending
                    };
                    break;
                default:
                    return BadRequest("Unknown entity type");
            }
        
        if (existingReport is not null && DateTime.UtcNow.Subtract(existingReport.DateTime).TotalDays < 1)
            return BadRequest("The entity has been recently reported by user");
        
        await FinishReportDataAndCreateAsync(report, authenticatedUserId, anonymousUserId, cancellationToken);
                    
        var apiReport = entityToDtoService.ReportEntityToDto(report);
        return Created(string.Empty, apiReport);
    }

    /// <summary>
    /// Get <see cref="ApiReport"/>s
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="amountPerPage">Amount of <see cref="ApiReport"/>s per page</param>
    /// <param name="status">Filter by <see cref="ReportStatus"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>A List of <see cref="ApiReport"/>s</returns>
    [HttpGet]
    [Authorize(Roles = "Moderator, Administrator")]
    public async Task<IActionResult> GetLatestReportsAsync([FromQuery] int page, [FromQuery] int amountPerPage, [FromQuery] ReportStatus? status,
        CancellationToken cancellationToken = default)
    {
        if (page < 1 || amountPerPage < 1)
            return BadRequest("Invalid pagination parameters");
        
        var query = (status == null) ?
            reportsRepository.GetLatest().Where(r => r.ReportStatus == ReportStatus.Pending)
            : reportsRepository.GetLatest().Where(r => r.ReportStatus == status);
        
        var reports = await query.Skip((page - 1) * amountPerPage)
            .Take(amountPerPage)
            .ToListAsync(cancellationToken);

        var apiReports = reports.Select(entityToDtoService.ReportEntityToDto).ToList();
        
        return Ok(apiReports);
    }

    /// <summary>
    /// Change <see cref="ApiReport"/>'s status
    /// </summary>
    /// <param name="id"><see cref="ApiReport"/> ID</param>
    /// <param name="command">The <see cref="ChangeReportStatusCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>Updated <see cref="ApiReport"/> object</returns>
    [HttpPatch("{id}")]
    [Authorize(Roles = "Moderator, Administrator")]
    public async Task<IActionResult> ChangeReportStatusAsync(int id, ChangeReportStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var report = await reportsRepository.GetByIdAsync(id, cancellationToken);
        if (report is null)
            return NotFound();
        
        report.ReportStatus = command.ReportStatus;
        reportsRepository.Update(report);
        
        await reportsRepository.SaveChangesAsync(cancellationToken);
        
        var apiReport = entityToDtoService.ReportEntityToDto(report);
        return Ok(apiReport);
    }

    /// <summary>
    /// Finish up a <see cref="Report"/> object and add that entity through the repository
    /// </summary>
    /// <param name="report">The <see cref="Report"/> to add onto</param>
    /// <param name="userId">The <see cref="User"/> ID</param>
    /// <param name="anonUserId">The <see cref="AnonymousUser"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="Task{int}"/></returns>
    /// <exception cref="ArgumentException"></exception>
    private Task<int> FinishReportDataAndCreateAsync(Report report, int? userId, Guid? anonUserId, 
        CancellationToken cancellationToken = default)
    {
        if (userId is null && anonUserId is null)
            throw new ArgumentException($"Either {nameof(userId)} or {nameof(anonUserId)} must be specified");
        if (userId != null && anonUserId != null)
            throw new ArgumentException(
                $"Both {nameof(userId)} and {nameof(anonUserId)} can't be specified at the same time");
        
        report.ReportingUserId = userId;
        report.ReportingAnonymousUserId = anonUserId;
        
        reportsRepository.Create(report);
        return reportsRepository.SaveChangesAsync(cancellationToken);
    }
}