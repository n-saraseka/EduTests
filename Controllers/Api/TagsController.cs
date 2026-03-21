using EduTests.ApiObjects;
using EduTests.Commands.TagCommands;
using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTests.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class TagsController(ITagRepository tagRepository): ControllerBase
{
    /// <summary>
    /// Create a <see cref="ApiTag"/>
    /// </summary>
    /// <param name="command">The <see cref="CreateTagCommand"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="CreatedAtActionResult"/> with the <see cref="ApiTag"/></returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddTagAsync([FromBody] CreateTagCommand command,
        CancellationToken cancellationToken = default)
    {
        var existingTag = await tagRepository.GetByNameAsync(command.Name, cancellationToken);
        if (existingTag is not null)
            return Conflict("Tag already exists");

        var tag = new Tag
        {
            Name = command.Name,
        };
        
        tagRepository.Create(tag);
        await tagRepository.SaveChangesAsync(cancellationToken);

        var apiTag = TagEntityToDto(tag);
        
        return CreatedAtAction("GetTag", new { id = tag.Id }, apiTag);
    }

    /// <summary>
    /// Get a <see cref="ApiTag"/>
    /// </summary>
    /// <param name="id">The <see cref="ApiTag"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>The <see cref="ApiTag"/></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTagAsync(int id, CancellationToken cancellationToken = default)
    {
        var tag = await tagRepository.GetByIdAsync(id, cancellationToken);
        if (tag is null)
            return NotFound();
        
        var apiTag = TagEntityToDto(tag);
        
        return Ok(apiTag);
    }

    /// <summary>
    /// Delete a <see cref="ApiTag"/>
    /// </summary>
    /// <param name="id">The <see cref="ApiTag"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="OkResult"/></returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Moderator, Administrator")]
    public async Task<IActionResult> DeleteTagAsync(int id, CancellationToken cancellationToken = default)
    {
        var tag = await tagRepository.GetByIdAsync(id, cancellationToken);
        if (tag is null)
            return NotFound();
        
        tagRepository.Delete(tag);
        await tagRepository.SaveChangesAsync(cancellationToken);
        
        return Ok();
    }

    private ApiTag TagEntityToDto(Tag tag)
    {
        var apiTag = new ApiTag
        {
            Id = tag.Id,
            Name = tag.Name
        };
        
        return apiTag;
    }
}