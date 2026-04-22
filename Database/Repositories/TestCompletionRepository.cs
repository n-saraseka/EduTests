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
    public Task<int> GetTestCompletionCountAsync(int id, CancellationToken cancellationToken = default) => 
        Set
            .Where(tc => tc.TestId == id && tc.CompletedAt != null)
            .CountAsync(cancellationToken);

    /// <summary>
    /// Get completion counts for multiple <see cref="Test"/>s
    /// </summary>
    /// <param name="ids"><see cref="Test"/> IDs</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>A dictionary with completion counts for each <see cref="Test"/></returns>
    public async Task<Dictionary<int, int>> GetTestCompletionCountsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var counts = await Set
            .Where(tc => ids.Contains(tc.TestId) && tc.CompletedAt != null)
            .GroupBy(tc => tc.TestId)
            .ToDictionaryAsync(g => g.Key, g => g.Count(), cancellationToken);
        
        foreach (var id in ids.Where(id => !counts.ContainsKey(id)))
            counts[id] = 0;
        
        return counts;
    }

    /// <summary>
    /// Get all <see cref="TestCompletion"/>s for this <see cref="Test"/> and <see cref="User"/> (or <see cref="AnonymousUser"/>)
    /// </summary>
    /// <param name="testId"><see cref="Test"/> ID</param>
    /// <param name="userId"><see cref="User"/> ID</param>
    /// <param name="anonymousUserId"><see cref="AnonymousUser"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>List of all corresponding <see cref="TestCompletion"/>s</returns>
    /// <exception cref="ArgumentException">If <paramref name="userId"/> and <paramref name="anonymousUserId"/> input is invalid</exception>
    public Task<List<TestCompletion>> GetByTestIdAndUserIdAsync(int testId, int? userId, Guid? anonymousUserId, 
        CancellationToken cancellationToken = default) {
        
        if (userId == null && anonymousUserId == null)
            throw new ArgumentException($"Either {nameof(userId)} or {nameof(anonymousUserId)} must be provided");

        if (userId != null && anonymousUserId != null)
            throw new ArgumentException(
                $"{nameof(userId)} and {nameof(anonymousUserId)} can't both be provided");
        
        if (userId != null)
            return Set.Where(tc => tc.TestId == testId && tc.UserId == userId).ToListAsync(cancellationToken);
        else
            return Set.Where(tc => tc.TestId == testId && tc.AnonymousUserId == anonymousUserId).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get a <see cref="TestCompletion"/> that hasn't been completed yet
    /// </summary>
    /// <param name="testId"><see cref="Test"/> ID</param>
    /// <param name="userId"><see cref="User"/> ID</param>
    /// <param name="anonymousUserId"><see cref="AnonymousUser"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>The corresponding <see cref="TestCompletion"/> or null</returns>
    /// <exception cref="ArgumentException">If <paramref name="userId"/> and <paramref name="anonymousUserId"/> input is invalid</exception>
    public Task<TestCompletion?> GetActiveCompletionAsync(int testId, int? userId, Guid? anonymousUserId,
        CancellationToken cancellationToken = default)
    {
        if (userId == null && anonymousUserId == null)
            throw new ArgumentException($"Either {nameof(userId)} or {nameof(anonymousUserId)} must be provided");
        
        if (userId != null && anonymousUserId != null)
            throw new ArgumentException(
                $"{nameof(userId)} and {nameof(anonymousUserId)} can't both be provided");
        
        if (userId != null)
            return Set.FirstOrDefaultAsync(tc => tc.TestId == testId && tc.UserId == userId && tc.CompletedAt == null, cancellationToken);
        else
            return Set.FirstOrDefaultAsync(tc => tc.TestId == testId && tc.AnonymousUserId == anonymousUserId && tc.CompletedAt == null, cancellationToken);
    }

    /// <summary>
    /// Get a <see cref="TestCompletion"/> with <see cref="User"/> and <see cref="Test"/> data
    /// </summary>
    /// <param name="id">The <see cref="TestCompletion"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="TestCompletion"/> or null</returns>
    public Task<TestCompletion?> GetWithExtendedDataAsync(int id, CancellationToken cancellationToken = default) => Set
        .Include(tc => tc.User)
        .Include(tc => tc.Test)
        .AsSplitQuery()
        .FirstOrDefaultAsync(tc => tc.Id == id, cancellationToken);
    
    /// <summary>
    /// Get all completed <see cref="TestCompletion"/>s for this <see cref="Test"/>
    /// </summary>
    /// <param name="id">The <see cref="Test" ID/></param>
    /// <returns>A <see cref="IQueryable"/> of corresponding <see cref="TestCompletion"/>s</returns>
    public IQueryable<TestCompletion> GetByTestId(int id) => Set
        .Include(tc => tc.User)
        .Include(tc => tc.Test)
        .AsSplitQuery()
        .Where(tc => tc.TestId == id && tc.CompletedAt != null);
}