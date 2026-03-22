using EduTests.Database.Entities;

namespace EduTests.Database.Repositories.Interfaces;

public interface IUserAnswerRepository : IRepository<UserAnswer, int>
{
    Task<List<UserAnswer>> GetByCompletionId(int id, CancellationToken cancellationToken);
}