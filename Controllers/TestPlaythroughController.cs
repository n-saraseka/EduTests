using System.Security.Claims;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Models;
using EduTests.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Controllers;

public class TestPlaythroughController(ITestRepository testRepository, 
    ICommentRepository commentRepository,
    IBannedUserRepository bannedUserRepository,
    ITestCompletionRepository testCompletionRepository,
    IUserAnswerRepository userAnswerRepository,
    IQuestionRepository questionRepository,
    IUserRatingRepository ratingRepository,
    IConfiguration config,
    IEntityToDtoService entityToDtoService) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> TestPlaythrough(int id, TestPlaythroughViewModel model,
        CancellationToken cancellationToken = default)
    {
        var completion = await testCompletionRepository.GetByIdAsync(id, cancellationToken);
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
        
        var test = await testRepository.GetByIdWithExtendedDataAsync(completion.TestId, cancellationToken);
        if (test == null) return NotFound("Test not found");

        model.Test = entityToDtoService.TestEntityToDto(test);

        var latestVersion = test.Questions
            .Where(q => q.UpdatedAt <= completion.StartedAt)
            .MaxBy(q => q.UpdatedAt)
            .UpdatedAt;
        
        var questions = test.Questions.Where(q => q.UpdatedAt == latestVersion).ToList();
        
        var apiQuestions = questions.Select(q =>
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
        
        model.Questions = apiQuestions;
        
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

    [AllowAnonymous]
    public async Task<IActionResult> TestResult(int id, TestResultViewModel viewModel,
        CancellationToken cancellationToken = default)
    {
        var completion = await testCompletionRepository.GetWithExtendedDataAsync(id, cancellationToken);
        if (completion is null) return NotFound("Playthrough not found");

        if (completion.CompletedAt is null) return BadRequest("Playthrough not completed yet");
        
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
            var currentRating = await ratingRepository.GetUsersRatingAsync(completion.TestId, authenticatedUserId, cancellationToken);
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
        
        var latestVersion = questions
            .Where(q => q.UpdatedAt <= completion.StartedAt)
            .MaxBy(q => q.UpdatedAt)
            .UpdatedAt;
        
        var relevantQuestions = questions.Where(q => q.UpdatedAt == latestVersion).ToList();
        
        var answers = await userAnswerRepository.GetByCompletionIdAsync(completion.Id, cancellationToken);
        
        var apiCompletion = entityToDtoService.CompletionEntityToDto(completion, answers, relevantQuestions);
        apiCompletion.Test = entityToDtoService.TestEntityToDto(completion.Test);
        if (completion.UserId != null)
        {
            apiCompletion.User = entityToDtoService.UserEntityToDto(completion.User);
        }
        
        viewModel.Completion = apiCompletion;
        
        var appropriateResult = completion.Test.Results
            .Where(r => r.PercentageThreshold <= apiCompletion.CompletionPercentage && r.UpdatedAt == latestVersion)
            .MaxBy(r => r.PercentageThreshold);

        if (appropriateResult != null) viewModel.ResultString = appropriateResult.Result;
        else if (completion.Test.DefaultResult != null) viewModel.ResultString = completion.Test.DefaultResult;
        
        var query = commentRepository.GetTestComments(completion.TestId);
        var pageSize = int.Parse(config["commentPageSize"]);
        viewModel.CommentsPerPage = pageSize;
        
        var count = await query.CountAsync(cancellationToken);
        viewModel.CommentPages = (int)Math.Ceiling((double)count / pageSize);
        
        var comments = await query.Take(pageSize).ToListAsync(cancellationToken);
        viewModel.Comments = comments.Select(entityToDtoService.CommentEntityToDto).ToList();
        
        return View(viewModel);
    }

    public async Task<IActionResult> ResultDetails(int id, ResultDetailsViewModel model,
        CancellationToken cancellationToken = default)
    {
        var completion = await testCompletionRepository.GetWithExtendedDataAsync(id, cancellationToken);
        if (completion is null) return NotFound("Playthrough not found");
        
        if (completion.CompletedAt is null) return BadRequest("Playthrough not completed yet");
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
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

        if (completion.Test.UserId != authenticatedUserId)
        {
            if ((authRes && completion.UserId != authenticatedUserId) 
                || (anonUserResult && completion.AnonymousUserId != anonId)) return Forbid();
        }
        if (!authRes && !anonUserResult) return BadRequest("Neither anonymous or authenticated user ID were assigned. WTF?");
        
        var questions = await questionRepository.GetByTestIdAsync(completion.TestId, cancellationToken);
        
        var latestVersion = questions
            .Where(q => q.UpdatedAt <= completion.StartedAt)
            .MaxBy(q => q.UpdatedAt)
            .UpdatedAt;
        
        var filteredQuestions = questions.Where(q => q.UpdatedAt == latestVersion).ToList();
        var apiQuestions = filteredQuestions.Select(entityToDtoService.QuestionEntityToDto).ToList();
        
        var answers = await userAnswerRepository.GetByCompletionIdAsync(completion.Id, cancellationToken);
        
        var apiCompletion = entityToDtoService.CompletionEntityToDto(completion, answers, filteredQuestions);
        apiCompletion.Test = entityToDtoService.TestEntityToDto(completion.Test);
        if (completion.UserId != null)
        {
            apiCompletion.User = entityToDtoService.UserEntityToDto(completion.User);
        }
        
        model.Completion = apiCompletion;
        
        model.Questions = apiQuestions.OrderBy(q => q.OrderIndex).ToList();
        model.Answers = answers.Select(entityToDtoService.AnswerEntityToDto).ToList();
        
        var appropriateResult = completion.Test.Results
            .Where(r => r.PercentageThreshold <= apiCompletion.CompletionPercentage && r.UpdatedAt == latestVersion)
            .MaxBy(r => r.PercentageThreshold);

        if (appropriateResult != null) model.ResultString = appropriateResult.Result;
        else if (completion.Test.DefaultResult != null) model.ResultString = completion.Test.DefaultResult;
        
        var pageSize = int.Parse(config["testDetailsPageSize"]);
        var pages = (int)Math.Ceiling((double)questions.Count / pageSize);
        
        model.PageSize = pageSize;
        model.Pages = pages;

        return View(model);
    }
}