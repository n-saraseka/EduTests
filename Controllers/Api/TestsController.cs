using System.Security.Claims;
using EduTests.ApiObjects;
using EduTests.Commands.TestCommands;
using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTests.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class TestsController(ITestRepository testRepository, 
    IUserRatingRepository ratingRepository, 
    ITestCompletionRepository testCompletionRepository) : ControllerBase
{
    /// <summary>
    /// Create a <see cref="ApiTest"/>
    /// </summary>
    /// <param name="command">The <see cref="CreateTestCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>A <see cref="ApiTest"/> object</returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateTest([FromBody] CreateTestCommand command, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();
        var id = int.Parse(userId);
        
        var test = new Test
        {
            UserId = id,
            Name = command.Name,
            Description = command.Description,
        };
        
        testRepository.Create(test);
        await testRepository.SaveChangesAsync(cancellationToken);

        var apiTest = new ApiTest
        {
            Id = test.Id,
            Name = test.Name,
            Description = test.Description,
            ThumbnailUrl = test.ThumbnailUrl,
            CreatedAt = test.CreatedAt,
            UpdatedAt = test.UpdatedAt
        };
        
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
        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test is null)
            return NotFound();

        var ratings = await ratingRepository.GetTestRatingAsync(id, cancellationToken);
        var completions = await testCompletionRepository.GetTestCompletionCountAsync(id, cancellationToken);
        var tags = test.Tags.Select(tag => tag.Name).ToList();
        
        var testToReturn = new ApiTest
        {
            Id = test.Id,
            Name = test.Name,
            Description = test.Description,
            ThumbnailUrl = test.ThumbnailUrl,
            Rating = ratings,
            CompletionCount = completions,
            CreatedAt = test.CreatedAt,
            UpdatedAt = test.UpdatedAt,
            AttemptLimit = test.AttemptLimit,
            TimeLimit = test.TimeLimit,
            Tags = tags,
        };
        
        return Ok(testToReturn);
    }

    /// <summary>
    /// Change the <see cref="ApiTest"/>'s details
    /// </summary>
    /// <param name="id"><see cref="ApiTest"/> ID</param>
    /// <param name="command">The <see cref="ChangeTestDetailsCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>Updated <see cref="ApiTest"/> object</returns>
    [HttpPut("{id}/details")]
    [Authorize]
    public async Task<IActionResult> ChangeTestDetailsAsync(int id, ChangeTestDetailsCommand command,
        CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test is null)
            return NotFound();
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();
        
        var userIdInt = int.Parse(userId);
        
        if (test.UserId != userIdInt)
            return Forbid();

        test.Description = command.Description;
        testRepository.Update(test);
        await testRepository.SaveChangesAsync(cancellationToken);
        
        var ratings = await ratingRepository.GetTestRatingAsync(id, cancellationToken);
        var completions = await testCompletionRepository.GetTestCompletionCountAsync(id, cancellationToken);
        var tags = test.Tags.Select(tag => tag.Name).ToList();
        
        var testToReturn = new ApiTest
        {
            Id = test.Id,
            Name = test.Name,
            Description = test.Description,
            ThumbnailUrl = test.ThumbnailUrl,
            Rating = ratings,
            CompletionCount = completions,
            CreatedAt = test.CreatedAt,
            UpdatedAt = test.UpdatedAt,
            AttemptLimit = test.AttemptLimit,
            TimeLimit = test.TimeLimit,
            Tags = tags,
        };
        
        return Ok(testToReturn);
    }
    
    
}