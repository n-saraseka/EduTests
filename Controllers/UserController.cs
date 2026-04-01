using System.Security.Claims;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Models;
using EduTests.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Controllers;

public class UserController(IUserRepository userRepository,
    ICommentRepository commentRepository,
    IReportsRepository reportsRepository,
    IBannedUserRepository bannedUserRepository,
    IEntityToDtoService entityToDtoService,
    IConfiguration config) : Controller
{
    public async Task<IActionResult> Profile(int id, ProfileViewModel model, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
            return NotFound();
        
        var apiUser = entityToDtoService.UserEntityToDto(user);
        model.User = apiUser;
        
        var commentQuery = commentRepository.GetProfileComments(id);
        var pageSize = int.Parse(config["pageSize"]);
        model.CommentsPerPage = pageSize;
        
        var commentCount = await commentQuery.CountAsync(cancellationToken);
        var pages = (int)Math.Ceiling((double)commentCount / pageSize);
        model.CommentPages = pages;
        
        var comments = await commentQuery
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        var apiComments = comments.Select(entityToDtoService.CommentEntityToDto).ToList();
        model.Comments = apiComments;
        
        var result = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);
        if (result)
            model.CurrentUserId = userId;
        
        var activeBan = await bannedUserRepository.GetUsersActiveBanAsync(id, cancellationToken);
        var isBanned = activeBan is not null;
        model.IsBanned = isBanned;

        if (result)
        {
            var activeBanCurr = await bannedUserRepository.GetUsersActiveBanAsync(userId, cancellationToken);
            var currBanned = activeBanCurr is not null;
            model.IsCurrentBanned = currBanned;
        }
        
        model.CurrentUserGroup = User.FindFirstValue(ClaimTypes.Role);

        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> Settings(int id, UserSettingsViewModel model,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
            return NotFound();
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();
        
        var userIdInt = int.Parse(userId);
        if (userIdInt != user.Id)
            return Forbid();
        
        var apiUser = entityToDtoService.UserEntityToDto(user);
        model.User = apiUser;

        if (User.FindFirstValue(ClaimTypes.Role) is "Moderator" or "Administrator")
        {
            var pageSize = int.Parse(config["pageSize"]);
            model.RowsPerTablePage = pageSize;

            var reportsQuery = reportsRepository.GetLatest();
            
            var reportCount = await reportsQuery.CountAsync(cancellationToken);
            var reportPages = (int)Math.Ceiling((double)reportCount / pageSize);
            model.ReportPages = reportPages;
            
            var reports = await reportsQuery.Take(pageSize).ToListAsync(cancellationToken);
            model.Reports = reports.Select(entityToDtoService.ReportEntityToDto).ToList();

            var bansQuery = bannedUserRepository.GetLatestBans(true);
            
            var banCount = await bansQuery.CountAsync(cancellationToken);
            var banPages = (int)Math.Ceiling((double)banCount / pageSize);
            model.BanPages = banPages;
            
            var bans = await bansQuery.Take(pageSize).ToListAsync(cancellationToken);
            model.Bans = bans.Select(entityToDtoService.BanEntityToDto).ToList();
        }
        
        return View(model);
    }
}