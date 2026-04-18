using System.Security.Claims;
using EduTests.Database.Enums;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Models;
using EduTests.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Controllers;

public class TestController(ITestRepository testRepository, 
    IUserRepository userRepository,
    ICommentRepository commentRepository,
    IBannedUserRepository bannedUserRepository,
    IConfiguration config,
    ITestStatsService testStatsService,
    IEntityToDtoService entityToDtoService) : Controller
{
    [Authorize]
    public async Task<IActionResult> Constructor(int id,
        ConstructorViewModel viewModel,
        CancellationToken cancellationToken = default)
    {
        var result = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);
        if (result)
        {
            var user = await userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                return BadRequest("User not found");
            viewModel.User = entityToDtoService.UserEntityToDto(user);
        }
        
        var test = await testRepository.GetByIdWithExtendedDataAsync(id, cancellationToken);
        if (test == null)
            return NotFound("Test not found");
        var apiTest = entityToDtoService.TestEntityToDto(test);
        apiTest.Questions = test.Questions.Select(entityToDtoService.QuestionEntityToDto).ToList();
        apiTest.Results = test.Results.Select(entityToDtoService.TestResultEntityToDto).ToList();
        viewModel.Test = apiTest;
        
        return View(viewModel);
    }

    public async Task<IActionResult> TestPage(int id, TestPageViewModel viewModel,
        CancellationToken cancellationToken = default)
    {
        viewModel.CurrentUserGroup = User.FindFirstValue(ClaimTypes.Role);
        var result = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);
        if (result)
        {
            viewModel.CurrentUserId = userId;
            var activeBanCurr = await bannedUserRepository.GetUsersActiveBanAsync(userId, cancellationToken);
            var currBanned = activeBanCurr is not null;
            viewModel.IsCurrentBanned = currBanned;
        }

        var test = await testRepository.GetByIdWithTagsAsync(id, cancellationToken);
        if (test == null) return NotFound("Test not found");
        if (test.AccessType == AccessType.Private && userId != test.UserId) return Forbid();
        
        var apiTest = entityToDtoService.TestEntityToDto(test);
        apiTest = await testStatsService.GetTestStatsAsync(apiTest, cancellationToken);
        
        viewModel.Test = apiTest;

        var query = commentRepository.GetTestComments(test.Id);
        var pageSize = int.Parse(config["commentPageSize"]);
        viewModel.CommentsPerPage = pageSize;
        
        var count = await query.CountAsync(cancellationToken);
        viewModel.CommentPages = (int)Math.Ceiling((double)count / pageSize);
        
        var comments = await query.Take(pageSize).ToListAsync(cancellationToken);
        viewModel.Comments = comments.Select(entityToDtoService.CommentEntityToDto).ToList();
        
        return View(viewModel);
    }
}