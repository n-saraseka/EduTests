using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;

namespace EduTests.Services;

public class UserAuthenticationService(IUserRepository repository) : IUserAuthenticationService
{
    public async Task<User?> ValidateUserAsync(string login, string password, CancellationToken cancellationToken)
    {
        var user = await repository.GetByLoginAsync(login, cancellationToken);
        if (user is null)
            return null;

        bool verified = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        return verified ? user : null;
    }

    public async Task RegisterAsync(string login, string password, string username,
        CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            Login = login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Username = username
        };
        repository.Create(user);
        await repository.SaveChangesAsync(cancellationToken);
    }
}