using EduTests.Database.Entities;

namespace EduTests.Database.Repositories.Interfaces;

public interface IUserAnswerRepository : IRepository<UserAnswer, int>
{
    Task<List<UserAnswer>> GetByCompletionIdAsync(int id, CancellationToken cancellationToken);
}