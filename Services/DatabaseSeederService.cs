using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;

namespace EduTests.Services;

public class DatabaseSeederService(IUserRepository userRepository, IConfiguration configuration) : IDatabaseSeederService
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedAdminAsync(cancellationToken);
    }
    
    private async Task SeedAdminAsync(CancellationToken cancellationToken = default)
    {
        if (await userRepository.GetByLoginAsync("admin", cancellationToken) == null)
        {
            var user = new User
            {
                Login = "admin",
                Username = "Administrator",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(configuration["adminPassword"]),
                RegistrationDate = DateTime.UtcNow
            };
            userRepository.Create(user);
            await userRepository.SaveChangesAsync(cancellationToken);
        }
    }
}