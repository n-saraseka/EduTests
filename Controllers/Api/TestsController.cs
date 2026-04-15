using System.Security.Claims;
using EduTests.ApiObjects;
using EduTests.Commands.CommentCommands;
using EduTests.Commands.TestCommands;
using EduTests.Database.Entities;
using EduTests.Database.Enums;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Services;
using EduTests.Services.Questions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class TestsController(ITestRepository testRepository, 
    IUserRatingRepository ratingRepository, 
    ITestCompletionRepository testCompletionRepository,
    ITagRepository tagRepository,
    IQuestionRepository questionRepository,
    ITestResultRepository testResultRepository,
    ICommentRepository commentRepository,
    IUserAnswerRepository userAnswerRepository,
    IAnonymousUserRepository anonymousUserRepository,
    IQuestionValidatorService questionValidatorService,
    IEntityToDtoService entityToDtoService) : ControllerBase
{
    /// <summary>
    /// Create a <see cref="ApiTest"/>
    /// </summary>
    /// <param name="command">The <see cref="CreateOrUpdateTestCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>A <see cref="ApiTest"/> object</returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateTest([FromBody] CreateOrUpdateTestCommand command, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();
        var id = int.Parse(userId);
        
        if (command.Questions.Count == 0)
            return BadRequest("No questions were added");
        
        var questions = command.Questions.Select(question => new Question
            {
                OrderIndex = question.OrderIndex,
                Type = question.Type,
                Description = question.Description,
                Data = question.Data,
                CorrectData = question.CorrectData,
            })
            .ToList();

        foreach (var question in questions)
            try
            {
                questionValidatorService.Validate(question.Data, question.CorrectData, question.Type);
            }
            catch (Exception)
            {
                return BadRequest("One or more questions are invalid");
            }
        
        var test = new Test
        {
            UserId = id,
            Name = command.Name,
            Description = command.Description,
            AttemptLimit = command.AttemptLimit,
            TimeLimit = command.TimeLimit,
            Password = command.Password
        };
        
        testRepository.Create(test);
        await testRepository.SaveChangesAsync(cancellationToken);

        questions = questions.Select(q =>
        {
            q.TestId = test.Id;
            return q;
        }).ToList();
        
        questionRepository.CreateBulk(questions);
        
        var tags = await tagRepository.GetByNameBulkAsync(command.Tags, cancellationToken);
        var newTags = command.Tags.Except(tags.Select(t => t.Name)).Select(t => t.ToLower()).Distinct();

        var tagsToAdd = newTags.Select(t => new Tag
        {
            Name = t
        });

        var allTags = tags.Concat(tagsToAdd).ToList();
        foreach (var tag in allTags)
            test.Tags.Add(tag);
        
        var results = command.Results.Select(r => new TestResult
        {
            TestId = test.Id,
            PercentageThreshold = r.PercentageThreshold,
            Result = r.Result
        });
        testResultRepository.CreateBulk(results);
        
        await testRepository.SaveChangesAsync(cancellationToken);

        var apiTest = entityToDtoService.TestEntityToDto(test);
        
        return CreatedAtAction("GetTest", new { id = apiTest.Id }, apiTest);
    }
    
    /// <summary>
    /// Get a <see cref="ApiTest"/>
    /// </summary>
    /// <param name="id"><see cref="ApiTest"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>A <see cref="ApiTest"/> object</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTestAsync(int id, CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdWithTagsAsync(id, cancellationToken);
        if (test is null)
            return NotFound();

        var testToReturn = entityToDtoService.TestEntityToDto(test);
        testToReturn.Rating = await ratingRepository.GetTestRatingAsync(test.Id, cancellationToken);
        testToReturn.CompletionCount = await testCompletionRepository.GetTestCompletionCountAsync(test.Id, cancellationToken);
        
        return Ok(testToReturn);
    }
    
    /// <summary>
    /// Get <see cref="ApiTest"/>s
    /// </summary>
    /// <param name="userId">The <see cref="ApiUser"/> ID</param>
    /// <param name="page">Page number</param>
    /// <param name="amountPerPage">Amount of <see cref="ApiTest"/>s per page</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>List of <see cref="ApiTest"/>s</returns>
    public async Task<IActionResult> GetTestsAsync(int? userId, int page, int amountPerPage, CancellationToken cancellationToken = default)
    {
        var query = testRepository.GetAll();
        if (userId != null)
        {
            query = testRepository.GetByUserId(userId.Value);
        }

        if (page < 1 || amountPerPage < 1) return BadRequest("Invalid pagination parameters");
        var count = await query.CountAsync(cancellationToken);
        var pages = (int)Math.Ceiling((double)count / amountPerPage);
        var actualPage = Math.Min(page, pages);
        query = query.Skip((actualPage - 1) * amountPerPage).Take(amountPerPage);

        var tests = await query.ToListAsync(cancellationToken);
        var apiTests = tests.Select(entityToDtoService.TestEntityToDto);
        var ids = apiTests.Select(t => t.Id);
        var ratings = await ratingRepository.GetTestRatingsAsync(ids, cancellationToken);
        var completions = await testCompletionRepository.GetTestCompletionCountsAsync(ids, cancellationToken);

        apiTests = apiTests.Select(t =>
        {
            t.Rating = ratings[t.Id];
            t.CompletionCount = completions[t.Id];
            return t;
        }).ToList();

        return Ok(new { tests = apiTests, pages });
    }

    /// <summary>
    /// Update a <see cref="ApiTest"/>
    /// </summary>
    /// <param name="id"><see cref="ApiTest"/> ID</param>
    /// <param name="command">The <see cref="CreateOrUpdateTestCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>Updated <see cref="ApiTest"/> object</returns>
    [HttpPatch("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateTestAsync(int id, [FromBody] CreateOrUpdateTestCommand command,
        CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdWithTagsAsync(id, cancellationToken);
        if (test is null)
            return NotFound();
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();
        
        var userIdInt = int.Parse(userId);
        
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        
        if (test.UserId != userIdInt && userRole != "Moderator" && userRole != "Administrator")
            return Forbid();

        test.Name = command.Name;
        test.Description = command.Description;
        test.AttemptLimit = command.AttemptLimit;
        test.TimeLimit = command.TimeLimit;
        test.Password = command.Password;
        
        if (command.Questions.Count == 0)
            return BadRequest("No questions were added");
        
        var questions = command.Questions.Select(question => new Question
            {
                OrderIndex = question.OrderIndex,
                Type = question.Type,
                Description = question.Description,
                Data = question.Data,
                CorrectData = question.CorrectData,
            })
            .ToList();

        foreach (var question in questions)
            try
            {
                questionValidatorService.Validate(question.Data, question.CorrectData, question.Type);
            }
            catch (Exception)
            {
                return BadRequest("One or more questions are invalid");
            }
        
        var existingQuestions = await questionRepository.GetByTestIdAsync(id, cancellationToken);
        questionRepository.DeleteBulk(existingQuestions);
        
        questions = questions.Select(q =>
        {
            q.TestId = test.Id;
            return q;
        }).ToList();
        
        questionRepository.CreateBulk(questions);
        
        var tags = await tagRepository.GetByNameBulkAsync(command.Tags, cancellationToken);
        var newTags = command.Tags.Except(tags.Select(t => t.Name)).Select(t => t.ToLower()).Distinct();

        var tagsToAdd = newTags.Select(t => new Tag
        {
            Name = t
        });
        
        var allTags = tags.Concat(tagsToAdd).ToList();
        
        test.Tags.Clear();
        foreach (var tag in allTags)
            test.Tags.Add(tag);
        
        var existingResults = await testResultRepository.GetByTestIdAsync(id, cancellationToken);
        testResultRepository.DeleteBulk(existingResults);
        
        var results = command.Results.Select(r => new TestResult
        {
            TestId = test.Id,
            PercentageThreshold = r.PercentageThreshold,
            Result = r.Result
        });
        testResultRepository.CreateBulk(results);
        
        await testRepository.SaveChangesAsync(cancellationToken);

        var apiTest = entityToDtoService.TestEntityToDto(test);

        return Ok(apiTest);
    }

    /// <summary>
    /// Delete a <see cref="ApiTest"/>
    /// </summary>
    /// <param name="id"><see cref="ApiTest"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkResult"/></returns>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteTestAsync(int id, CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test is null)
            return NotFound();
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();
        
        var userIdInt = int.Parse(userId);
        
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        
        if (test.UserId != userIdInt && userRole != "Moderator" && userRole != "Administrator")
            return Forbid();
        
        testRepository.Delete(test);
        await testRepository.SaveChangesAsync(cancellationToken);
        
        return Ok();
    }

    /// <summary>
    /// Rate a <see cref="ApiTest"/> as current user
    /// </summary>
    /// <param name="id"><see cref="ApiTest"/> ID</param>
    /// <param name="command">The <see cref="RateTestCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>A <see cref="ApiRating"/> object</returns>
    [HttpPut("{id}/rating")]
    [Authorize]
    public async Task<IActionResult> RateTestAsync(int id, RateTestCommand command, 
        CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test is null)
            return NotFound();
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();
        
        var userIdInt = int.Parse(userId);
        
        if (test.AccessType == AccessType.Private && test.UserId != userIdInt)
            return Forbid();
        
        var existingRating = await ratingRepository.GetUsersRatingAsync(test.Id, userIdInt, cancellationToken);

        if (existingRating is null)
        {
            var rating = new UserRating
            {
                TestId = test.Id,
                UserId = userIdInt,
                IsPositive = command.IsPositive
            };
            
            ratingRepository.Create(rating);
            await ratingRepository.SaveChangesAsync(cancellationToken);

            var apiRating = entityToDtoService.RatingEntityToDto(rating);
            return CreatedAtAction("GetTestRating", new { id = test.Id }, apiRating);
        }
        else
        {
            existingRating.IsPositive = command.IsPositive;
            ratingRepository.Update(existingRating);
            await ratingRepository.SaveChangesAsync(cancellationToken);
        
            var apiRating = entityToDtoService.RatingEntityToDto(existingRating);
            return Ok(apiRating);
        }
    }

    /// <summary>
    /// Get current user's <see cref="ApiTest"/> rating
    /// </summary>
    /// <param name="id"><see cref="ApiTest"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>A <see cref="ApiRating"/> object</returns>
    [HttpGet("{id}/rating")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTestRatingAsync(int id, CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test is null)
            return NotFound();
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();
        
        var userIdInt = int.Parse(userId);
        
        if (test.AccessType == AccessType.Private && test.UserId != userIdInt)
            return Forbid();
        
        var rating = await ratingRepository.GetUsersRatingAsync(test.Id, userIdInt, cancellationToken);
        if (rating is null)
            return NotFound();
        
        var apiRating = entityToDtoService.RatingEntityToDto(rating);
        return Ok(apiRating);
    }

    /// <summary>
    /// Create a <see cref="ApiComment"/> on an <see cref="ApiTest"/>
    /// </summary>
    /// <param name="id">The <see cref="ApiTest"/> ID</param>
    /// <param name="command">The <see cref="CreateCommentCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="CreatedAtActionResult"/> with the <see cref="ApiComment"/></returns>
    [HttpPost("{id}/comments")]
    [Authorize]
    public async Task<IActionResult> CreateTestCommentAsync(int id, [FromBody] CreateCommentCommand command, 
        CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test is null)
            return NotFound();
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();
        
        var userIdInt = int.Parse(userId);
        
        if (test.AccessType == AccessType.Private && test.UserId != userIdInt)
            return Forbid();

        var comment = new Comment
        {
            CommenterId = userIdInt,
            TestId = id,
            Content = command.Content
        };
        
        commentRepository.Create(comment);
        await commentRepository.SaveChangesAsync(cancellationToken);
        
        comment = await commentRepository.GetWithLoadedCommenter(comment.Id, cancellationToken);

        var apiComment = entityToDtoService.CommentEntityToDto(comment);
        return CreatedAtAction("GetTestComment", new { id = test.Id, commentId = comment.Id }, apiComment);
    }

    /// <summary>
    /// Get a <see cref="ApiComment"/> on a <see cref="ApiTest"/>
    /// </summary>
    /// <param name="id">The <see cref="ApiTest"/> ID</param>
    /// <param name="commentId">The <see cref="ApiComment"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>A <see cref="ApiComment"/> object</returns>
    [HttpGet("{id}/comments/{commentId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTestCommentAsync(int id, int commentId,
        CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test is null)
            return NotFound();
        
        if (test.AccessType == AccessType.Private)
            return Forbid();
        
        var comment = await commentRepository.GetWithLoadedCommenter(id, cancellationToken);
        
        if (comment is null)
            return NotFound();

        var apiComment = entityToDtoService.CommentEntityToDto(comment);
        
        return Ok(apiComment);
    }

    /// <summary>
    /// Get <see cref="ApiComment"/>s on an <see cref="ApiTest"/>
    /// </summary>
    /// <param name="id">The <see cref="ApiTest"/> ID</param>
    /// <param name="page">Page number</param>
    /// <param name="amountPerPage">Amount of <see cref="ApiComment"/>s per page</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>List of <see cref="ApiComment"/>s</returns>
    [HttpGet("{id}/comments")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTestCommentsAsync(int id, [FromQuery] int page, [FromQuery] int amountPerPage, 
        CancellationToken cancellationToken = default)
    {
        if (page < 1 || amountPerPage < 1)
            return BadRequest("Invalid pagination parameters");
        
        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test is null)
            return NotFound();
        
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        
        if (test.AccessType == AccessType.Private && userRole != "Administrator")
            return Forbid();

        var query = commentRepository.GetTestComments(id);

        var commentCount = await query.CountAsync(cancellationToken);
        var pages = Math.Ceiling((double)commentCount / amountPerPage);
        
        var comments = await query.OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * amountPerPage)
            .Take(amountPerPage)
            .ToListAsync(cancellationToken);
        
        var apiComments = comments.Select(entityToDtoService.CommentEntityToDto).ToList();
        
        return Ok(new {comments = apiComments, pages});
    }

    /// <summary>
    /// Delete a <see cref="ApiComment"/> on an <see cref="ApiTest"/>
    /// </summary>
    /// <param name="id">The <see cref="ApiTest"/> ID</param>
    /// <param name="commentId">The <see cref="ApiComment"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkResult"/></returns>
    [HttpDelete("{id}/comments/{commentId}")]
    [Authorize]
    public async Task<IActionResult> DeleteTestCommentAsync(int id, int commentId,
        CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test is null)
            return NotFound();
        
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        
        if (test.AccessType == AccessType.Private && userRole != "Administrator")
            return Forbid();
 
        var comment = await commentRepository.GetByIdAsync(commentId, cancellationToken);
        if (comment is null)
            return NotFound();
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();
        
        var userIdInt = int.Parse(userId);
        
        if (comment.CommenterId != userIdInt && userRole != "Moderator" && userRole != "Administrator")
            return Forbid();
        
        commentRepository.Delete(comment);
        await commentRepository.SaveChangesAsync(cancellationToken);

        return Ok();
    }

    /// <summary>
    /// Get <see cref="ApiQuestion"/>s from a <see cref="ApiTest"/>
    /// </summary>
    /// <param name="id">The <see cref="ApiTest"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>A List of <see cref="ApiQuestion"/>s</returns>
    [HttpGet("{id}/questions")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTestQuestionsAsync(int id, CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test is null)
            return NotFound();
        
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        
        if (test.AccessType == AccessType.Private && userRole != "Administrator")
            return Forbid();
        
        var questions = await questionRepository.GetByTestIdAsync(id, cancellationToken);
        
        if (questions.Count == 0)
            return NoContent();

        var apiQuestions = questions.Select(entityToDtoService.QuestionEntityToDto).ToList();
        
        return Ok(apiQuestions);
    }

    /// <summary>
    /// Create a <see cref="ApiTest"/>'s <see cref="ApiCompletion"/>
    /// </summary>
    /// <param name="id">The <see cref="ApiTest"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>Created <see cref="ApiCompletion"/></returns>
    [HttpPost("{id}/completions")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateTestCompletionAsync(int id, CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test is null)
            return NotFound();
        
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
        
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        
        if (test.AccessType == AccessType.Private && test.UserId != authenticatedUserId && userRole != "Moderator" && userRole != "Administrator")
            return Forbid();
        
        var existingCompletions = await testCompletionRepository
            .GetByTestIdAndUserIdAsync(id, authenticatedUserId, anonymousUserId, cancellationToken);
        if (existingCompletions.Count == test.AttemptLimit)
            return Forbid();
        
        var pendingCompletion = existingCompletions.FirstOrDefault(tc => tc.CompletedAt == null);
        if (pendingCompletion is not null)
            return BadRequest("There is already a uncompleted completion");

        var completion = new TestCompletion
        {
            TestId = id,
            StartedAt = DateTime.UtcNow
        };
        
        if (isAuthenticated)
            completion.UserId = authenticatedUserId;
        else
            completion.AnonymousUserId = anonymousUserId;
        
        var apiCompletion = entityToDtoService.CompletionEntityToDto(completion, null, null);
        return CreatedAtAction("GetTestCompletion", new { id = test.Id, completionId = completion.Id }, apiCompletion);
    }

    /// <summary>
    /// Get a <see cref="ApiTest"/>'s <see cref="ApiCompletion"/>
    /// </summary>
    /// <param name="id">The <see cref="ApiTest"/> ID</param>
    /// <param name="completionId">The <see cref="ApiCompletion"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>The <see cref="ApiCompletion"/></returns>
    [HttpGet("{id}/completions/{completionId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTestCompletionAsync(int id, int completionId,
        CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test is null)
            return NotFound();
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAuthenticated = User.Identity?.IsAuthenticated ?? false;

        int? authenticatedUserId = isAuthenticated ? int.Parse(userId) : null;
        Guid? anonymousUserId = !isAuthenticated ? Guid.Parse(userId) : null;
        
        var completion = await testCompletionRepository.GetByIdAsync(completionId, cancellationToken);
        if (completion is null)
            return NotFound();

        if (completion.UserId != authenticatedUserId && completion.AnonymousUserId != anonymousUserId)
            return Forbid();
        
        var apiCompletion = entityToDtoService.CompletionEntityToDto(completion, null, null);
        return Ok(apiCompletion);
    }

    /// <summary>
    /// Finish a <see cref="ApiCompletion"/>
    /// </summary>
    /// <param name="id">The <see cref="ApiTest"/> ID</param>
    /// <param name="completionId">The <see cref="ApiCompletion"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>Updated <see cref="ApiCompletion"/></returns>
    [HttpPatch("{id}/completions/{completionId}")]
    [AllowAnonymous]
    public async Task<IActionResult> FinishTestCompletionAsync(int id, int completionId,
        CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test is null)
            return NotFound();
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAuthenticated = User.Identity?.IsAuthenticated ?? false;

        int? authenticatedUserId = isAuthenticated ? int.Parse(userId) : null;
        Guid? anonymousUserId = !isAuthenticated ? Guid.Parse(userId) : null;
        
        var completion = await testCompletionRepository.GetByIdAsync(completionId, cancellationToken);
        if (completion is null)
            return NotFound();

        if (completion.UserId != authenticatedUserId && completion.AnonymousUserId != anonymousUserId)
            return Forbid();
        
        if (completion.CompletedAt != null)
            return BadRequest("The completion is already finished");
        
        var questions = await questionRepository.GetByTestIdAsync(id, cancellationToken);
        var userAnswers = await userAnswerRepository.GetByCompletionId(completionId, cancellationToken);
        
        if (questions.Count != userAnswers.Count)
            return BadRequest("Not all questions have been answered");
        
        completion.CompletedAt = DateTime.UtcNow;
        testCompletionRepository.Update(completion);
        await testCompletionRepository.SaveChangesAsync(cancellationToken);
        
        var apiCompletion =  entityToDtoService.CompletionEntityToDto(completion, null, null);
        return Ok(apiCompletion);
    }

    /// <summary>
    /// Get specific <see cref="ApiAnswer"/> for this <see cref="ApiTest"/>'s <see cref="ApiCompletion"/>
    /// </summary>
    /// <param name="id">The <see cref="ApiTest"/> ID</param>
    /// <param name="completionId">The <see cref="ApiCompletion"/> ID</param>
    /// <param name="answerId">The <see cref="ApiAnswer"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>A <see cref="ApiAnswer"/> object</returns>
    [HttpGet("{id}/completions/{completionId}/answers/{answerId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTestAnswerAsync(int id, int completionId, int answerId,
        CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAuthenticated = User.Identity?.IsAuthenticated ?? false;

        int? authenticatedUserId = isAuthenticated ? int.Parse(userId) : null;
        Guid? anonymousUserId = !isAuthenticated ? Guid.Parse(userId) : null;
        
        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test is null)
            return NotFound();
        
        var completion = await testCompletionRepository.GetByIdAsync(completionId, cancellationToken);
        if (completion is null)
            return NotFound();
        
        if (completion.UserId != authenticatedUserId && completion.AnonymousUserId != anonymousUserId)
            return Forbid();
        
        var answer = await userAnswerRepository.GetByIdAsync(answerId, cancellationToken);
        if (answer is null)
            return NotFound();

        var apiAnswer = entityToDtoService.AnswerEntityToDto(answer);
        
        return Ok(apiAnswer);
    }
    
    /// <summary>
    /// Add a <see cref="ApiAnswer"/> to this <see cref="ApiTest"/>'s <see cref="ApiCompletion"/>
    /// </summary>
    /// <param name="id">The <see cref="ApiTest"/> ID</param>
    /// <param name="completionId">The <see cref="ApiCompletion"/> ID</param>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{id}/completions/{completionId}/answers/")]
    [AllowAnonymous]
    public async Task<IActionResult> AddTestAnswerAsync(int id, int completionId, 
        [FromBody] AnswerTestCommand command, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAuthenticated = User.Identity?.IsAuthenticated ?? false;

        int? authenticatedUserId = isAuthenticated ? int.Parse(userId) : null;
        Guid? anonymousUserId = !isAuthenticated ? Guid.Parse(userId) : null;
        
        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test is null)
            return NotFound();
        
        var completion = await testCompletionRepository.GetByIdAsync(completionId, cancellationToken);
        if (completion is null)
            return NotFound();
        
        if (completion.UserId != authenticatedUserId && completion.AnonymousUserId != anonymousUserId)
            return Forbid();
        
        if (completion.CompletedAt != null)
            return BadRequest("The completion is already answered");
        
        var questions = await questionRepository.GetByTestIdAsync(id, cancellationToken);
        if (questions.Any(q => q.Id == command.QuestionId))
            return BadRequest("The question is already answered");

        if (questions.All(q => q.Id != command.QuestionId))
            return NotFound();
        
        var answers = await userAnswerRepository.GetByCompletionId(completionId, cancellationToken);
        if (answers.Count == questions.Count)
            return BadRequest("All  questions have been answered");

        var answer = new UserAnswer
        {
            TestCompletionId = completionId,
            QuestionId = command.QuestionId,
            Answers = command.Answer
        };
        
        userAnswerRepository.Create(answer);
        await userAnswerRepository.SaveChangesAsync(cancellationToken);

        var apiAnswer = entityToDtoService.AnswerEntityToDto(answer);
        
        return CreatedAtAction("GetTestAnswer", new
        {
            id = test.Id, completionId = completion.Id, answerId = answer.Id
        }, apiAnswer);
    }

    /// <summary>
    /// Edit a <see cref="ApiAnswer"/> to this <see cref="ApiTest"/>'s <see cref="ApiCompletion"/>
    /// </summary>
    /// <param name="id">The <see cref="ApiTest"/> ID</param>
    /// <param name="completionId">The <see cref="ApiCompletion"/> ID</param>
    /// <param name="answerId">The <see cref="ApiAnswer"/> ID</param>
    /// <param name="command">The <see cref="EditTestAnswerCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>Updated <see cref="ApiAnswer"/> object</returns>
    [HttpPatch("{id}/completions/{completionId}/answers/{answerId}")]
    [AllowAnonymous]
    public async Task<IActionResult> EditAnswerAsync(int id, int completionId, int answerId,
        [FromBody] EditTestAnswerCommand command, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAuthenticated = User.Identity?.IsAuthenticated ?? false;

        int? authenticatedUserId = isAuthenticated ? int.Parse(userId) : null;
        Guid? anonymousUserId = !isAuthenticated ? Guid.Parse(userId) : null;
        
        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test is null)
            return NotFound();
        
        var completion = await testCompletionRepository.GetByIdAsync(completionId, cancellationToken);
        if (completion is null)
            return NotFound();
        
        if (completion.UserId != authenticatedUserId && completion.AnonymousUserId != anonymousUserId)
            return Forbid();
        
        if (completion.CompletedAt != null)
            return BadRequest("The completion is already answered");
        
        var answer = await userAnswerRepository.GetByIdAsync(answerId, cancellationToken);
        if (answer is null)
            return NotFound();
        
        var questions = await questionRepository.GetByTestIdAsync(id, cancellationToken);

        if (questions.All(q => q.Id != answer.QuestionId))
            return NotFound();

        answer.Answers = command.NewAnswer;
        userAnswerRepository.Update(answer);
        await userAnswerRepository.SaveChangesAsync(cancellationToken);
        
        var apiAnswer = entityToDtoService.AnswerEntityToDto(answer);
        return Ok(apiAnswer);
    }
}