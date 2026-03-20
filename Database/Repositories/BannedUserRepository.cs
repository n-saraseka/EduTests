using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Services;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class BannedUserRepository(DatabaseContext db) : BaseRepository<BannedUser, int>(db), IBannedUserRepository
{
    public Task<BannedUser?> GetUsersActiveBanAsync(int userId, CancellationToken cancellationToken)
    {
        return Set.Where(b =>
                b.UserBannedId == userId
                && (b.DateUnbanned <= DateTime.UtcNow || b.DateUnbanned == null))
            .FirstOrDefaultAsync(cancellationToken);
    }
}