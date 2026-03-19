namespace EduTests.Services;

public interface IDatabaseSeederService
{
    Task SeedAsync(CancellationToken cancellationToken);
}