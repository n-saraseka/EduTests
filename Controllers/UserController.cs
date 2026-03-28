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
        
        model.IsAuthorized = HttpContext.User.Identity?.IsAuthenticated ?? false;

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
        
        return View(model);
    }
}