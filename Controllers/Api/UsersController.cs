using EduTests.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTests.Controllers.Api;

public class UsersController(IUserRepository userRepository) : Controller
{
    public async Task<IActionResult> TryLoginAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByLoginAsync(login, cancellationToken);
        if (user == null)
            return Unauthorized();
        var verified = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!verified)
            return Unauthorized();
        return Ok(user);
    }
}