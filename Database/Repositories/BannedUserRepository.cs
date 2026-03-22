using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using EduTests.Services;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class BannedUserRepository(DatabaseContext db) : BaseRepository<BannedUser, int>(db), IBannedUserRepository
{
    /// <summary>
    /// Get this <see cref="User"/>'s active <see cref="BannedUser"/> object
    /// </summary>
    /// <param name="userId"><see cref="User"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="BannedUser"/> object or null</returns>
    public Task<BannedUser?> GetUsersActiveBanAsync(int userId, CancellationToken cancellationToken)
    {
        return Set.Where(b =>
                b.UserBannedId == userId
                && (b.DateUnbanned <= DateTime.UtcNow || b.DateUnbanned == null))
            .FirstOrDefaultAsync(cancellationToken);
    }
}