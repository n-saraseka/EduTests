using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;

namespace EduTests.Services;

public class AuthentificationService(IUserRepository repository) : IAuthentificationService
{
    public async Task<User?> ValidateUserAsync(string login, string password, CancellationToken cancellationToken)
    {
        var user = await repository.GetByLoginAsync(login, cancellationToken);
        if (user is null)
            return null;

        bool verified = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        return verified ? user : null;
    }
}