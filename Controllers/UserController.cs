using EduTests.ApiObjects;
using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Models;
using Microsoft.AspNetCore.Mvc;

namespace EduTests.Controllers;

public class UserController(IUserRepository userRepository) : Controller
{
    public async Task<IActionResult> Profile(int id, ProfileViewModel model, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
            return NotFound();
        
        var apiUser = UserEntityToDto(user);
        model.User = apiUser;

        return View(model);
    }
    
    /// <summary>
    /// Map <see cref="User"/> entity to <see cref="ApiUser"/> DTO
    /// </summary>
    /// <param name="entity">The <see cref="User"/> entity</param>
    /// <returns>The <see cref="ApiUser"/> DTO</returns>
    private ApiUser UserEntityToDto(User entity)
    {
        var apiUser = new ApiUser
        {
            Id = entity.Id,
            Username = entity.Username,
            AvatarUrl = entity.AvatarUrl,
            Description = entity.Description,
            RegistrationDate = entity.RegistrationDate,
            Group = entity.Group
        };
        
        return apiUser;
    }
}