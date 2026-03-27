using EduTests.ApiObjects;
using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Models;
using EduTests.Services;
using Microsoft.AspNetCore.Mvc;

namespace EduTests.Controllers;

public class UserController(IUserRepository userRepository,
    IEntityToDtoService entityToDtoService) : Controller
{
    public async Task<IActionResult> Profile(int id, ProfileViewModel model, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
            return NotFound();
        
        var apiUser = entityToDtoService.UserEntityToDto(user);
        model.User = apiUser;

        return View(model);
    }
}