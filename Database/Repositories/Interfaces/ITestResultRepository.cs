using EduTests.Database.Entities;

namespace EduTests.Database.Repositories.Interfaces;

public interface ITestResultRepository : IRepository<TestResult, int>
{ 
    Task<List<TestResult>> GetByTestIdAsync(int testId, CancellationToken cancellationToken);
}