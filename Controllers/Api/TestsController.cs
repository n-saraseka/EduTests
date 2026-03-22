using System.Security.Claims;
using EduTests.ApiObjects;
using EduTests.Commands.TestCommands;
using EduTests.Database.Entities;
using EduTests.Database.Enums;
using EduTests.Database.Repositories.Interfaces;
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
    ICommentRepository commentRepository,
    IUserAnswerRepository userAnswerRepository,
    IQuestionValidatorService questionValidatorService,
    IAnswerVerifierService answerVerifierService) : ControllerBase
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
        
        var questions = command.Questions.Select(question => new Question
            {
                TestId = test.Id,
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
                questionValidatorService.Validate(question.Data, question.CorrectData, question.Type, true);
            }
            catch (Exception)
            {
                return BadRequest("One or more questions are invalid");
            }
        
        questionRepository.CreateBulk(questions);
        
        var tags = await tagRepository.GetByNameBulkAsync(command.Tags, cancellationToken);
        var newTags = command.Tags.Except(tags.Select(t => t.Name));

        var tagsToAdd = newTags.Select(t => new Tag
        {
            Name = t
        });
        
        tagRepository.CreateBulk(tagsToAdd);

        var allTags = tags.Concat(tagsToAdd).ToList();
        foreach (var tag in allTags)
            test.Tags.Add(tag);
        await testRepository.SaveChangesAsync(cancellationToken);
        
        var apiTest = await TestEntityToDto(test, cancellationToken);
        
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

        var testToReturn = await TestEntityToDto(test, cancellationToken);
        
        return Ok(testToReturn);
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

        if (command.Questions.Count > 0)
        {
            var questions = command.Questions.Select(question => new Question
                {
                    TestId = test.Id,
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
                    questionValidatorService.Validate(question.Data, question.CorrectData, question.Type, true);
                }
                catch (Exception)
                {
                    return BadRequest("One or more questions are invalid");
                }
        
            var existingQuestions = await questionRepository.GetByTestIdAsync(id, cancellationToken);
            questionRepository.DeleteBulk(existingQuestions);
            questionRepository.CreateBulk(questions);
        }
        
        var tags = await tagRepository.GetByNameBulkAsync(command.Tags, cancellationToken);
        var newTags = command.Tags.Where(t => 
            tags.Select(tag => tag.Name).Contains(t)).ToList();

        var tagsToAdd = newTags.Select(t => new Tag
        {
            Name = t
        });
        
        tagRepository.CreateBulk(tagsToAdd);
        
        var allTags = tags.Concat(tagsToAdd).ToList();
        
        test.Tags.Clear();
        foreach (var tag in allTags)
            test.Tags.Add(tag);
        
        await testRepository.SaveChangesAsync(cancellationToken);

        var apiTest = await TestEntityToDto(test, cancellationToken);

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

            var apiRating = RatingEntityToDto(rating);
            return CreatedAtAction("GetTestRating", new { id = test.Id }, apiRating);
        }
        else
        {
            existingRating.IsPositive = command.IsPositive;
            ratingRepository.Update(existingRating);
            await ratingRepository.SaveChangesAsync(cancellationToken);
        
            var apiRating = RatingEntityToDto(existingRating);
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
        
        var apiRating = RatingEntityToDto(rating);
        return Ok(apiRating);
    }

    /// <summary>
    /// Create a <see cref="ApiComment"/> on an <see cref="ApiTest"/>
    /// </summary>
    /// <param name="id">The <see cref="ApiTest"/> ID</param>
    /// <param name="command">The <see cref="CreateTestCommentCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="CreatedAtActionResult"/> with the <see cref="ApiComment"/></returns>
    [HttpPost("{id}/comments")]
    [Authorize]
    public async Task<IActionResult> CreateTestCommentAsync(int id, [FromBody] CreateTestCommentCommand command, 
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
        
        var apiComment = CommentEntityToDto(comment);
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
        
        var comment = await commentRepository.GetByIdAsync(commentId, cancellationToken);
        
        if (comment is null)
            return NotFound();
        
        var apiComment = CommentEntityToDto(comment);
        
        return Ok(apiComment);
    }

    /// <summary>
    /// Get <see cref="ApiComment"/>s on an <see cref="ApiTest"/>
    /// </summary>
    /// <param name="id">The <see cref="ApiTest"/> ID</param>
    /// <param name="page">Page number</param>
    /// <param name="amountPerPage">Amount of <see cref="ApiReport"/>s per page</param>
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
        
        var comments = await query.OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * amountPerPage)
            .Take(amountPerPage)
            .ToListAsync(cancellationToken);
        
        var apiComments = comments.Select(CommentEntityToDto).ToList();
        
        return Ok(apiComments);
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

        var apiQuestions = questions.Select(QuestionEntityToDto).ToList();
        
        return Ok(apiQuestions);
    }

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
        
        var apiCompletion = CompletionEntityToDto(completion, null, null, cancellationToken);
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
        
        var apiCompletion = CompletionEntityToDto(completion, null, null, cancellationToken);
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
        
        var apiCompletion =  CompletionEntityToDto(completion, userAnswers, questions, cancellationToken);
        return Ok(apiCompletion);
    }
    
    /// <summary>
    /// Map <see cref="Test"/> entity to <see cref="ApiTest"/> DTO
    /// </summary>
    /// <param name="entity">The <see cref="Test"/> entity</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>The <see cref="ApiTest"/> DTO</returns>
    private async Task<ApiTest> TestEntityToDto(Test entity, CancellationToken cancellationToken)
    {
        var ratings = await ratingRepository.GetTestRatingAsync(entity.Id, cancellationToken);
        var completions = await testCompletionRepository.GetTestCompletionCountAsync(entity.Id, cancellationToken);
        var tags = entity.Tags.Select(t => t.Name).ToList();
        
        var testToReturn = new ApiTest
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            ThumbnailUrl = entity.ThumbnailUrl,
            Rating = ratings,
            CompletionCount = completions,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            AttemptLimit = entity.AttemptLimit,
            TimeLimit = entity.TimeLimit,
            Tags = tags,
        };
        
        return testToReturn;
    }

    /// <summary>
    /// Map <see cref="UserRating"/> entity to <see cref="ApiRating"/> DTO
    /// </summary>
    /// <param name="entity">The <see cref="UserRating"/> entity</param>
    /// <returns><see cref="ApiRating"/> DTO</returns>
    private ApiRating RatingEntityToDto(UserRating entity)
    {
        var ratingToReturn = new ApiRating
        {
            TestId = entity.TestId,
            UserId = entity.UserId,
            IsPositive = entity.IsPositive
        };
        
        return ratingToReturn;
    }

    /// <summary>
    /// Map <see cref="Comment"/> entity to <see cref="ApiComment"/> DTO
    /// </summary>
    /// <param name="entity">The <see cref="Comment"/> entity</param>
    /// <returns>The <see cref="ApiComment"/> DTO</returns>
    /// <exception cref="ArgumentNullException">In case <see cref="Comment.UserProfileId"/> and <see cref="Comment.TestId"/> both are null</exception>
    private ApiComment CommentEntityToDto(Comment entity)
    {
        var entityType = (entity.UserProfileId != null) ? CommentEntityType.UserProfile : CommentEntityType.Test;
        var entityId = entity.UserProfileId ?? entity.TestId;
        
        if (entityId is null)
            throw new ArgumentNullException(nameof(entityId));

        var commentToReturn = new ApiComment
        {
            Id = entity.Id,
            UserId = entity.CommenterId,
            EntityType = entityType,
            EntityId = (int)entityId,
            Content = entity.Content,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };

        return commentToReturn;
    }

    /// <summary>
    /// Map a <see cref="Question"/> entity to <see cref="ApiQuestion"/> DTO
    /// </summary>
    /// <param name="entity">The <see cref="Question"/> entity</param>
    /// <returns>The <see cref="ApiQuestion"/> DTO</returns>
    private ApiQuestion QuestionEntityToDto(Question entity)
    {
        var questionToReturn = new ApiQuestion
        {
            Id = entity.Id,
            TestId = entity.TestId,
            OrderIndex = entity.OrderIndex,
            Type = entity.Type,
            Description = entity.Description,
            Data = entity.Data,
            CorrectData = entity.CorrectData,
        };
        
        return questionToReturn;
    }

    /// <summary>
    /// Convert <see cref="TestCompletion"/> entity to <see cref="ApiCompletion"/> DTO
    /// </summary>
    /// <param name="entity">The <see cref="TestCompletion"/> entity</param>
    /// <param name="userAnswers">List of <see cref="UserAnswer"/>s (if the completion has finished)</param>
    /// <param name="questions">List of <see cref="Question"/>s (if the completion has finished)</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>The <see cref="ApiCompletion"/> DTO</returns>
    /// <exception cref="ArgumentNullException">If there's no corresponding <see cref="Question"/> for a <see cref="UserAnswer"/></exception>
    private ApiCompletion CompletionEntityToDto(TestCompletion entity, List<UserAnswer>? userAnswers, List<Question>? questions,
        CancellationToken cancellationToken = default)
    {
        var userId = entity.UserId;
        var anonymousId = entity.AnonymousUserId;
        
        if (userId is null && anonymousId is null)
            throw new ArgumentException($"Neither {nameof(entity.UserId)} or {nameof(entity.AnonymousUserId)} are not null");
        
        var completionToReturn = new ApiCompletion
        {
            Id = entity.Id,
            TestId = entity.TestId,
            UserId = userId,
            StartedAt = entity.StartedAt,
            CompletedAt = entity.CompletedAt,
        };
        
        if (entity.CompletedAt is not null)
        {
            var questionCount = questions.Count;
            var correctAnswers = 0;
            var correctPercentage = 0.0;

            foreach (var answer in userAnswers)
            {
                var correspondingQuestion = questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (correspondingQuestion is null)
                    throw new ArgumentNullException(nameof(correspondingQuestion));
                if (answerVerifierService.Verify(answer, correspondingQuestion, correspondingQuestion.Type))
                    correctAnswers++;
            }
            
            completionToReturn.CorrectAnswers = correctAnswers;
            correctPercentage = correctAnswers * 100.0 / questionCount;
            completionToReturn.CompletionPercentage = Math.Round(correctPercentage, 2);
        }

        return completionToReturn;
    }
}