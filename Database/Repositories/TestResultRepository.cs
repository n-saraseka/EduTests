using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class TestResultRepository(DatabaseContext db) : BaseRepository<TestResult, int>(db), ITestResultRepository
{
    /// <summary>
    /// Get all <see cref="TestResult"/>s by respective <see cref="Test"/> ID
    /// </summary>
    /// <param name="testId">The <see cref="Test"/>'s id</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>List with all corresponding <see cref="TestResult"/>s</returns>
    public Task<List<TestResult>> GetByTestIdAsync(int testId, CancellationToken cancellationToken) =>
    Set.Where(r => r.TestId == testId).ToListAsync(cancellationToken);
}