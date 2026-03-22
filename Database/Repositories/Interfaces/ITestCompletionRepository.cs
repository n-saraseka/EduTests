using EduTests.Database.Entities;

namespace EduTests.Database.Repositories.Interfaces;

public interface ITestCompletionRepository : IRepository<TestCompletion, int>
{
    Task<int> GetTestCompletionCountAsync(int id, CancellationToken cancellationToken);
    Task<Dictionary<int, int>> GetTestCompletionCountsAsync(IEnumerable<int> ids, CancellationToken cancellationToken);
    Task<List<TestCompletion>> GetByTestIdAndUserIdAsync(int testId, int userId, CancellationToken cancellationToken);
}