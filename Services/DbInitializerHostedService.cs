using EduTests.Database;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Services;

public class DbInitializerHostedService(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
        var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeederService>();
        await seeder.SeedAsync(cancellationToken);
    }
    
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}