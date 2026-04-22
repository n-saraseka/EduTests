using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class UserAnswerRepository(DatabaseContext db) : BaseRepository<UserAnswer, int>(db), IUserAnswerRepository
{
    /// <summary>
    /// Get all <see cref="UserAnswer"/>s for respective <see cref="TestCompletion"/>
    /// </summary>
    /// <param name="id">The <see cref="TestCompletion"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>A List of all <see cref="UserAnswer"/>s</returns>
    public Task<List<UserAnswer>> GetByCompletionIdAsync(int id, CancellationToken cancellationToken)
    {
        return Set.Where(ua => ua.TestCompletionId == id).ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// Get all <see cref="UserAnswer"/>s for corresponding <see cref="TestCompletion"/>s
    /// </summary>
    /// <param name="ids">A <see cref="IEnumerable{int}"/> containing <see cref="TestCompletion"/> IDs</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>A dictionary with <see cref="UserAnswer"/>s for each <see cref="TestCompletion"/></returns>
    public async Task<Dictionary<int, List<UserAnswer>>> GetByCompletionIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var answers = await Set
            .Where(ua => ids.Contains(ua.TestCompletionId))
            .GroupBy(ua => ua.TestCompletionId)
            .ToDictionaryAsync(g => g.Key, g => g.ToList(), cancellationToken);
        return answers;
    }
}