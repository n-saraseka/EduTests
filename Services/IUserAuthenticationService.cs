using EduTests.Database.Entities;

namespace EduTests.Services;

public interface IUserAuthenticationService
{
    Task<User?> ValidateUserAsync(string login, string password, CancellationToken cancellationToken = default);
    Task<User?> RegisterAsync(string login, string password, string username, CancellationToken cancellationToken = default);
}