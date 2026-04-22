using EduTests.Database.Entities;

namespace EduTests.Database.Repositories.Interfaces;

public interface ITestCompletionRepository : IRepository<TestCompletion, int>
{
    Task<int> GetTestCompletionCountAsync(int id, CancellationToken cancellationToken);
    Task<Dictionary<int, int>> GetTestCompletionCountsAsync(IEnumerable<int> ids, CancellationToken cancellationToken);
    Task<List<TestCompletion>> GetByTestIdAndUserIdAsync(int testId, int? userId, Guid? anonymousUserId,
        CancellationToken cancellationToken = default);
    Task<TestCompletion?> GetActiveCompletionAsync(int testId, int? userId, Guid? anonymousUserId,
        CancellationToken cancellationToken = default);
    Task<TestCompletion?> GetWithExtendedDataAsync(int id, CancellationToken cancellationToken = default);
    public IQueryable<TestCompletion> GetByTestId(int id);
}