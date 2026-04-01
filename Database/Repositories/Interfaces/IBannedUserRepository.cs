using EduTests.Database.Entities;

namespace EduTests.Database.Repositories.Interfaces;

public interface IBannedUserRepository : IRepository<BannedUser, int>
{
    public Task<BannedUser?> GetUsersActiveBanAsync(int userId, CancellationToken cancellationToken);
    IQueryable<BannedUser> GetLatestBans(bool isActive);
}