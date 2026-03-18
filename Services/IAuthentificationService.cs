using EduTests.Database.Entities;

namespace EduTests.Services;

public interface IAuthentificationService
{
    Task<User?> ValidateUserAsync(string login, string password, CancellationToken cancellationToken = default);
}