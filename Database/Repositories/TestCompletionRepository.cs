using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class TestCompletionRepository(DatabaseContext db) : BaseRepository<TestCompletion, int>(db), ITestCompletionRepository
{
    /// <summary>
    /// Get a <see cref="Test"/>'s completion count
    /// </summary>
    /// <param name="id"><see cref="Test"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>The <see cref="Test"/>'s completion count</returns>
    public async Task<int> GetTestCompletionCountAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Set
            .Where(tc => tc.TestId == id)
            .CountAsync(cancellationToken);
    }

    /// <summary>
    /// Get completion counts for multiple <see cref="Test"/>s
    /// </summary>
    /// <param name="ids"><see cref="Test"/> IDs</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>A dictionary with completion counts for each <see cref="Test"/></returns>
    public async Task<Dictionary<int, int>> GetTestCompletionCountsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var counts = await Set
            .Where(tc => ids.Contains(tc.TestId))
            .GroupBy(tc => tc.TestId)
            .Select(g => new { TestId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TestId, x => x.Count, cancellationToken);
        
        foreach (var id in ids.Where(id => !counts.ContainsKey(id)))
            counts[id] = 0;
        
        return counts;
    }
}