using EduTests.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Controllers.Api;

[ApiController]
public class TestsController(ITestRepository testRepository, 
    IUserRatingRepository ratingRepository, 
    ITestCompletionRepository testCompletionRepository) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetTestAsync(int id, CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test == null)
            return NotFound();
        var rating = await ratingRepository.GetTestRatingAsync(id, cancellationToken);
        var completions = await testCompletionRepository.GetTestCompletionCountAsync(id, cancellationToken);
        return Ok(new {test, rating, completions});
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> SearchAsync(string text, int page, CancellationToken cancellationToken = default)
    {
        var query = testRepository.Search(text);
        var count = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * 20).Take(20).ToListAsync(cancellationToken);
        var ids = items.Select(t => t.Id);
        var ratings = await ratingRepository.GetTestRatingsAsync(ids, cancellationToken);
        var completions = await testCompletionRepository.GetTestCompletionCountsAsync(ids, cancellationToken);
        return Ok(new {count, items, ratings, completions});
    }
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetByTagAsync(string tag, int page, CancellationToken cancellationToken = default)
    {
        var query = testRepository.GetAllByTag(tag);
        var count = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * 20).Take(20).ToListAsync(cancellationToken);
        var ids = items.Select(t => t.Id);
        var ratings = await ratingRepository.GetTestRatingsAsync(ids, cancellationToken);
        var completions = await testCompletionRepository.GetTestCompletionCountsAsync(ids, cancellationToken);
        return Ok(new {count, items, ratings, completions});
    }
}