using System.Security.Claims;
using EduTests.ApiObjects;
using EduTests.Commands.TestCommands;
using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTests.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class TestsController(ITestRepository testRepository, 
    IUserRatingRepository ratingRepository, 
    ITestCompletionRepository testCompletionRepository,
    ITagRepository tagRepository,
    IQuestionRepository questionRepository,
    IQuestionValidatorService questionValidatorService) : ControllerBase
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
    public async Task<IActionResult> UpdateTestAsync(int id, CreateOrUpdateTestCommand command,
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
}