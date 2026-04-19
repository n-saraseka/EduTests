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

    public async Task<IActionResult> TestPlaythrough(int id, int playthroughId, TestPlaythroughViewModel model,
        CancellationToken cancellationToken = default)
    {
        var completion = await testCompletionRepository.GetByIdAsync(playthroughId, cancellationToken);
        if (completion is null) return NotFound("Playthrough not found");
        
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

    public async Task<IActionResult> TestResult(int id, int playthroughId, TestResultModel model,
        CancellationToken cancellationToken = default)
    {
        var completion = await testCompletionRepository.GetWithExtendedDataAsync(playthroughId, cancellationToken);
        if (completion is null) return NotFound("Playthrough not found");
        
        var questions = await questionRepository.GetByTestIdAsync(completion.TestId, cancellationToken);
        var answers = await userAnswerRepository.GetByCompletionIdAsync(completion.Id, cancellationToken);
        
        var apiCompletion = entityToDtoService.CompletionEntityToDto(completion, answers, questions);
        apiCompletion.Test = entityToDtoService.TestEntityToDto(completion.Test);
        if (completion.UserId != null)
        {
            apiCompletion.User = entityToDtoService.UserEntityToDto(completion.User);
        }
        
        model.Completion = apiCompletion;
        
        model.CurrentUserGroup = User.FindFirstValue(ClaimTypes.Role);
        var result = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);
        if (result)
        {
            model.CurrentUserId = userId;
            var activeBanCurr = await bannedUserRepository.GetUsersActiveBanAsync(userId, cancellationToken);
            var currBanned = activeBanCurr is not null;
            model.IsCurrentBanned = currBanned;
        }
        
        var query = commentRepository.GetTestComments(id);
        var pageSize = int.Parse(config["commentPageSize"]);
        model.CommentsPerPage = pageSize;
        
        var count = await query.CountAsync(cancellationToken);
        model.CommentPages = (int)Math.Ceiling((double)count / pageSize);
        
        var comments = await query.Take(pageSize).ToListAsync(cancellationToken);
        model.Comments = comments.Select(entityToDtoService.CommentEntityToDto).ToList();
        
        return View(model);
    }
}