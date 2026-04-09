using System.Security.Claims;
using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Models;
using EduTests.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTests.Controllers;

public class ConstructorController(IUserRepository userRepository, IEntityToDtoService entityToDtoService) : Controller
{
    [Authorize]
    public async Task<IActionResult> BaseConstructor(ConstructorViewModel viewModel, 
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
        
        return View(viewModel);
    }
}