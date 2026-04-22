using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class UserRatingRepository(DatabaseContext db) : BaseRepository<UserRating, int>(db), IUserRatingRepository
{
    /// <summary>
    /// Calculate a <see cref="Test"/>'s rating
    /// </summary>
    /// <param name="id"><see cref="Test"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>The <see cref="Test"/>'s rating</returns>
    public async Task<int> GetTestRatingAsync(int id, CancellationToken cancellationToken)
    {
        var query = Set.Where(r => r.TestId == id);
        
        var positive = await query.CountAsync(r => r.IsPositive, cancellationToken);
        var negative = await query.CountAsync(r => !r.IsPositive, cancellationToken);

        return positive - negative;
    }

    /// <summary>
    /// Calculate ratings for multiple <see cref="Test"/>s
    /// </summary>
    /// <param name="ids"><see cref="Test"/> IDs</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>A dictionary with ratings for each <see cref="Test"/></returns>
    public async Task<Dictionary<int, int>> GetTestRatingsAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
    {
        var counts = await Set
            .Where(r => ids.Contains(r.TestId))
            .GroupBy(r => r.TestId)
            .ToDictionaryAsync(g => g.Key, g => g.Sum(r => r.IsPositive ? 1 : -1), cancellationToken);
        
        foreach (var id in ids.Where(id => !counts.ContainsKey(id)))
            counts[id] = 0;
        
        return counts;
    }

    /// <summary>
    /// Get <see cref="User"/>'s <see cref="UserRating"/> of this <see cref="Test"/>
    /// </summary>
    /// <param name="testId"><see cref="Test"/> ID</param>
    /// <param name="userId"><see cref="User"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>Corresponding <see cref="UserRating"/> or null</returns>
    public Task<UserRating?> GetUsersRatingAsync(int testId, int userId, CancellationToken cancellationToken = default)
    {
        return Set.FirstOrDefaultAsync(r => r.TestId == testId && r.UserId == userId, cancellationToken);
    }
}