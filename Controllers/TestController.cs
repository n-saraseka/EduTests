using System.Security.Claims;
using EduTests.Database.Entities;
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
    ITestCompletionRepository testCompletionRepository,
    IUserAnswerRepository userAnswerRepository,
    IQuestionRepository questionRepository,
    IConfiguration config,
    ITestStatsService testStatsService,
    IEntityToDtoService entityToDtoService) : Controller
{
    [Authorize]
    public async Task<IActionResult> Constructor(int id,
        ConstructorViewModel viewModel,
        CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdWithExtendedDataAsync(id, cancellationToken);
        if (test == null)
            return NotFound("Test not found");
        
        var result = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);
        if (result)
        {
            if (test.UserId != userId) return Forbid();
            var user = await userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                return BadRequest("User not found");
            viewModel.User = entityToDtoService.UserEntityToDto(user);
        }
        else return Unauthorized();
        
        var apiTest = entityToDtoService.TestEntityToDto(test);
        
        var latestVersion = test.Questions
            .MaxBy(q => q.UpdatedAt)
            .UpdatedAt;

        var relevantQuestions = test.Questions.Where(q => q.UpdatedAt == latestVersion);
        var relevantResults = test.Results.Where(r => r.UpdatedAt == latestVersion);
        
        apiTest.Questions = relevantQuestions.Select(entityToDtoService.QuestionEntityToDto).ToList();
        apiTest.Results = relevantResults.Select(entityToDtoService.TestResultEntityToDto).ToList();
        viewModel.Test = apiTest;
        
        return View(viewModel);
    }

    [AllowAnonymous]
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
        
        var pageSize = int.Parse(config["commentPageSize"]);
        viewModel.CommentsPerPage = pageSize;

        if (test.Password != null)
        {
            var existingCookie = HttpContext.Request.Cookies[$"Verified-{test.Id}"];
            if (!string.IsNullOrEmpty(existingCookie))
            {
                var query = commentRepository.GetTestComments(test.Id);
                var count = await query.CountAsync(cancellationToken);
                viewModel.CommentPages = (int)Math.Ceiling((double)count / pageSize);

                var comments = await query.Take(pageSize).ToListAsync(cancellationToken);
                viewModel.Comments = comments.Select(entityToDtoService.CommentEntityToDto).ToList();
            }
            else viewModel.CanStartTest = false;
        }
        else
        {
            var query = commentRepository.GetTestComments(test.Id);
            var count = await query.CountAsync(cancellationToken);
            viewModel.CommentPages = (int)Math.Ceiling((double)count / pageSize);

            var comments = await query.Take(pageSize).ToListAsync(cancellationToken);
            viewModel.Comments = comments.Select(entityToDtoService.CommentEntityToDto).ToList();
        }
        
        return View(viewModel);
    }

    [Authorize]
    public async Task<IActionResult> TestStats(int id, TestStatisticsViewModel viewModel, CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdWithTagsAsync(id, cancellationToken);
        if (test == null) return NotFound("Test not found");
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var authRes = int.TryParse(userId, out var authenticatedUserId);
        if (!(authRes && test.UserId == authenticatedUserId)) return Forbid();
        
        var apiTest = entityToDtoService.TestEntityToDto(test);
        apiTest.User = entityToDtoService.UserEntityToDto(test.User);
        
        viewModel.Test = entityToDtoService.TestEntityToDto(test);
        
        var questions = await questionRepository.GetByTestIdAsync(id, cancellationToken);
        
        var allVersions = questions.DistinctBy(q => q.UpdatedAt).Select(q => q.UpdatedAt);
        var latestVersion = allVersions.Max();
        viewModel.Versions = allVersions.OrderDescending().ToList();

        var relevantQuestions = questions.Where(q => q.UpdatedAt == latestVersion).ToList();
        
        var completions = await testCompletionRepository.GetByTestId(id).ToListAsync(cancellationToken);
        var relevantCompletions = completions.Where(c => c.StartedAt >= latestVersion);
        var ids = relevantCompletions.Select(c => c.Id);
        var answers = await userAnswerRepository.GetByCompletionIdsAsync(ids, cancellationToken);
        
        viewModel.Questions = relevantQuestions.Select(entityToDtoService.QuestionEntityToDto).ToList();

        var apiCompletions = relevantCompletions.Select(c =>
            {
                var completion = entityToDtoService.CompletionEntityToDto(c, answers[c.Id], relevantQuestions);
                if (completion.UserId != null) completion.User = entityToDtoService.UserEntityToDto(c.User);
                return completion;
            });
        
        var count = apiCompletions.Count();
        viewModel.CompletionStats.CompletionCount = count;
        if (count > 0)
        {
            var orderedByPercentage = apiCompletions
                .OrderBy(c => c.CompletionPercentage)
                .Select(c => c.CompletionPercentage)
                .ToList();
            
            viewModel.CompletionStats.MinPercentage = orderedByPercentage[0];
            viewModel.CompletionStats.MaxPercentage = orderedByPercentage[count - 1];

            var midpoint = count / 2;
            if (count % 2 == 0)
                viewModel.CompletionStats.MedianPercentage = (orderedByPercentage.ElementAt(midpoint - 1) + orderedByPercentage.ElementAt(midpoint)) / 2;
            else
                viewModel.CompletionStats.MedianPercentage = orderedByPercentage.ElementAt(midpoint);
            
            var orderedByTime = apiCompletions
                .OrderBy(c => c.CompletedAt - c.StartedAt)
                .Select(c => (DateTime)c.CompletedAt - c.StartedAt)
                .ToList();
            
            viewModel.CompletionStats.MinTime = orderedByTime[0];
            viewModel.CompletionStats.MaxTime = orderedByTime[count - 1];
            
            var quarter = (int)Math.Round(count / 4.0,  MidpointRounding.AwayFromZero);
            var half = (int)Math.Round(count / 2.0,  MidpointRounding.AwayFromZero);
            
            var interQuartile = count > 3 ? orderedByTime.Take(half).Skip(quarter).ToList() : orderedByTime.ToList();
            viewModel.CompletionStats.InterQuartileAverageTime = new TimeSpan((long)interQuartile.Average(t => t.Ticks));
        }
        
        var pageSize = int.Parse(config["testStatisticsRows"]);
        viewModel.PageSize = pageSize;
        viewModel.Pages = (int)Math.Ceiling((double)count / pageSize);
        
        viewModel.Completions = apiCompletions.Take(pageSize).ToList();
        
        return View(viewModel);
    }
}