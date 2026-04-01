using EduTests.ApiObjects;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class BansController(IBannedUserRepository bannedUserRepository, 
    IEntityToDtoService entityToDtoService) : ControllerBase
{
    /// <summary>
    /// Get the latest <see cref="ApiBan"/>s
    /// </summary>
    /// <param name="active">Whether to filter for active bans only or not</param>
    /// <param name="page">Page number</param>
    /// <param name="amountPerPage">Amount of <see cref="ApiReport"/>s per page</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>A List of <see cref="ApiBan"/>s, as well as the amount of pages</returns>
    [HttpGet]
    [Authorize(Roles = "Moderator, Administrator")]
    public async Task<IActionResult> GetLatestBansAsync([FromQuery] bool active, [FromQuery] int page, 
        [FromQuery] int amountPerPage, CancellationToken cancellationToken = default)
    {
        if (page < 1 || amountPerPage < 1)
            return BadRequest("Invalid pagination parameters");
        var query = bannedUserRepository.GetLatestBans(active);
        
        var banCount = await query.CountAsync(cancellationToken);
        var pages = Math.Ceiling((double)banCount / amountPerPage);
        
        var bans = await query
            .Skip((page - 1) * amountPerPage)
            .Take(amountPerPage)
            .ToListAsync(cancellationToken);

        var apiBans = bans.Select(entityToDtoService.BanEntityToDto).ToList();

        return Ok(new { bans = apiBans, pages });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Moderator, Administrator")]
    public async Task<IActionResult> DeleteBanAsync(int id, CancellationToken cancellationToken = default)
    {
        var ban = await bannedUserRepository.GetByIdAsync(id, cancellationToken);
        if (ban is null)
            return NotFound();
        
        bannedUserRepository.Delete(ban);
        await bannedUserRepository.SaveChangesAsync(cancellationToken);
        
        return Ok();
    }
}