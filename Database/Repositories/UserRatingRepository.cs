using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class UserRatingRepository(DatabaseContext db) : BaseRepository<UserRating, int>(db), IUserRatingRepository
{
    public async Task<int> ComputeTestRatingAsync(int testId, CancellationToken cancellationToken)
    {
        var positiveCount = await Set.CountAsync(r => r.TestId == testId && r.IsPositive, cancellationToken);
        var negativeCount = await Set.CountAsync(r => r.TestId == testId && !r.IsPositive, cancellationToken);
        
        return positiveCount - negativeCount;
    }
}