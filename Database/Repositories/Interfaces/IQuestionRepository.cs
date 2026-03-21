using EduTests.Database.Entities;

namespace EduTests.Database.Repositories.Interfaces;

public interface IQuestionRepository : IRepository<Question, int>
{
    Task<List<Question>> GetByTestIdAsync(int testId, CancellationToken cancellationToken);
}