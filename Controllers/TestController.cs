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
    IUserRatingRepository ratingRepository,
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

    public async Task<IActionResult> TestPlaythrough(int id, int playthroughId, TestPlaythroughViewModel model,
        CancellationToken cancellationToken = default)
    {
        var completion = await testCompletionRepository.GetByIdAsync(playthroughId, cancellationToken);
        if (completion is null) return NotFound("Playthrough not found");

        if (completion.CompletedAt != null) return Forbid();
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAuthenticated = User.Identity?.IsAuthenticated ?? false;

        int? authenticatedUserId = isAuthenticated ? int.Parse(userId) : null;
        var anonUserResult = HttpContext.Items.TryGetValue("AnonymousId", out var anonIdObj);
        Guid? anonId = null;
        
        if (!isAuthenticated && anonUserResult && anonIdObj is string anonIdStr)
        {
            if (Guid.TryParse(anonIdStr, out var anonymousUserId))
            {
                anonId = anonymousUserId;
            }
        }

        if (completion.UserId != authenticatedUserId && completion.AnonymousUserId != anonId) return Forbid();
        
        model.Completion = entityToDtoService.CompletionEntityToDto(completion, null, null);
        
        var test = await testRepository.GetByIdWithExtendedDataAsync(id, cancellationToken);
        if (test == null) return NotFound("Test not found");

        model.Test = entityToDtoService.TestEntityToDto(test);
        
        var questions = test.Questions.Select(q =>
        {
            // this is pretty ugly, maybe refactor so that this uses a service instead
            q.CorrectData.ChosenIndices.Clear();
            q.CorrectData.LeftColumn.Clear();
            q.CorrectData.RightColumn.Clear();
            q.CorrectData.Options.Clear();
            q.CorrectData.Pairs.Clear();
            q.CorrectData.Sequence.Clear();
            q.CorrectData.NumberAnswer = null;
            q.CorrectData.Tolerance = null;
            q.CorrectData.TextAnswer = null;
            q.CorrectData.ValidAnswers.Clear();
            
            return entityToDtoService.QuestionEntityToDto(q);
        }).ToList();
        
        model.Questions = questions;
        
        var answers = await userAnswerRepository.GetByCompletionIdAsync(completion.Id, cancellationToken);
        model.Answers = answers.Select(entityToDtoService.AnswerEntityToDto).ToList();
        
        // we need to retrieve the latest unanswered question order index to save progress
        if (answers.Count > 0)
        {
            var correspondingAnsweredQuestions = questions.Where(q => 
                answers.Select(a => a.QuestionId).Contains(q.Id));
            var lastOrderIndex = correspondingAnsweredQuestions.Max(q => q.OrderIndex);
            model.LastUnansweredQuestion = lastOrderIndex != questions.Count ? lastOrderIndex + 1 : lastOrderIndex;
        }
        return View(model);
    }

    public async Task<IActionResult> TestResult(int id, int playthroughId, TestResultViewModel viewModel,
        CancellationToken cancellationToken = default)
    {
        var completion = await testCompletionRepository.GetWithExtendedDataAsync(playthroughId, cancellationToken);
        if (completion is null) return NotFound("Playthrough not found");
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        viewModel.CurrentUserGroup = User.FindFirstValue(ClaimTypes.Role);
        var authRes = int.TryParse(userId, out var authenticatedUserId);
        var anonUserResult = HttpContext.Items.TryGetValue("AnonymousId", out var anonIdObj);
        Guid? anonId = null;
        
        if (anonUserResult && anonIdObj is string anonIdStr)
        {
            if (Guid.TryParse(anonIdStr, out var anonymousUserId))
            {
                anonId = anonymousUserId;
            }
        }
        
        if (authRes)
        {
            if (completion.UserId != authenticatedUserId) return Forbid();
            viewModel.CurrentUserId = authenticatedUserId;
            var activeBanCurr = await bannedUserRepository.GetUsersActiveBanAsync(authenticatedUserId, cancellationToken);
            var currBanned = activeBanCurr is not null;
            viewModel.IsCurrentBanned = currBanned;
            var currentRating = await ratingRepository.GetUsersRatingAsync(id, authenticatedUserId, cancellationToken);
            if (currentRating != null)
                viewModel.CurrentRating = entityToDtoService.RatingEntityToDto(currentRating);
        }
        else
        {
            if (anonId != null)
            {
                if (completion.AnonymousUserId != anonId) return Forbid();
            }
            else return BadRequest("Neither anonymous or authenticated user ID were assigned. WTF?");
        }

        if (completion.UserId != authenticatedUserId && completion.AnonymousUserId != anonId) return Forbid();
        
        var questions = await questionRepository.GetByTestIdAsync(completion.TestId, cancellationToken);
        var answers = await userAnswerRepository.GetByCompletionIdAsync(completion.Id, cancellationToken);
        
        var apiCompletion = entityToDtoService.CompletionEntityToDto(completion, answers, questions);
        apiCompletion.Test = entityToDtoService.TestEntityToDto(completion.Test);
        if (completion.UserId != null)
        {
            apiCompletion.User = entityToDtoService.UserEntityToDto(completion.User);
        }
        
        viewModel.Completion = apiCompletion;
        
        var query = commentRepository.GetTestComments(id);
        var pageSize = int.Parse(config["commentPageSize"]);
        viewModel.CommentsPerPage = pageSize;
        
        var count = await query.CountAsync(cancellationToken);
        viewModel.CommentPages = (int)Math.Ceiling((double)count / pageSize);
        
        var comments = await query.Take(pageSize).ToListAsync(cancellationToken);
        viewModel.Comments = comments.Select(entityToDtoService.CommentEntityToDto).ToList();
        
        return View(viewModel);
    }

    [Authorize]
    public async Task<IActionResult> TestStats(int id, TestStatisticsViewModel viewModel, CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdWithTagsAsync(id, cancellationToken);
        if (test == null) return NotFound("Test not found");
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var authRes = int.TryParse(userId, out var authenticatedUserId);
        if (authRes && test.UserId != authenticatedUserId) return Forbid();
        
        var apiTest = entityToDtoService.TestEntityToDto(test);
        apiTest.User = entityToDtoService.UserEntityToDto(test.User);
        
        viewModel.Test = entityToDtoService.TestEntityToDto(test);
        
        var completions = await testCompletionRepository.GetByTestId(id).ToListAsync(cancellationToken);
        var ids = completions.Select(c => c.Id);
        var answers = await userAnswerRepository.GetByCompletionIdsAsync(ids, cancellationToken);
        var questions = await questionRepository.GetByTestIdAsync(id, cancellationToken);

        var apiCompletions = completions.Select(c =>
            {
                var completion = entityToDtoService.CompletionEntityToDto(c, answers[c.Id], questions);
                if (completion.UserId != null) completion.User = entityToDtoService.UserEntityToDto(c.User);
                return completion;
            });
        
        var count = apiCompletions.Count();
        if (count > 0)
        {
            var orderedByPercentage = apiCompletions
                .OrderBy(c => c.CompletionPercentage)
                .Select(c => c.CompletionPercentage)
                .ToList();
            
            viewModel.MinPercentage = orderedByPercentage[0];
            viewModel.MaxPercentage = orderedByPercentage[count - 1];

            var midpoint = count / 2;
            if (count % 2 == 0)
                viewModel.MedianPercentage = (orderedByPercentage.ElementAt(midpoint - 1) + orderedByPercentage.ElementAt(midpoint)) / 2;
            else
                viewModel.MedianPercentage = orderedByPercentage.ElementAt(midpoint);
            
            var orderedByTime = apiCompletions
                .OrderBy(c => c.CompletedAt - c.StartedAt)
                .Select(c => (DateTime)c.CompletedAt - c.StartedAt)
                .ToList();
            
            viewModel.MinTime = orderedByTime[0];
            viewModel.MaxTime = orderedByTime[count - 1];
            
            var quarter = (int)Math.Round(count / 4.0,  MidpointRounding.AwayFromZero);
            var half = (int)Math.Round(count / 2.0,  MidpointRounding.AwayFromZero);
            
            var interQuartile = count > 3 ? orderedByTime.Take(half).Skip(quarter).ToList() : orderedByTime.ToList();
            viewModel.InterQuartileAverageTime = new TimeSpan((long)interQuartile.Average(t => t.Ticks));
        }
        
        var pageSize = int.Parse(config["testStatisticsRows"]);
        viewModel.PageSize = pageSize;
        viewModel.Pages = (int)Math.Ceiling((double)count / pageSize);
        
        viewModel.Completions = apiCompletions.Take(pageSize).ToList();
        
        return View(viewModel);
    }
}