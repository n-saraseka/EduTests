using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class UserAnswerRepository(DatabaseContext db) : BaseRepository<UserAnswer, int>(db), IUserAnswerRepository
{
    public Task<List<UserAnswer>> GetByCompletionId(int id, CancellationToken cancellationToken)
    {
        return Set.Where(ua => ua.TestCompletionId == id).ToListAsync(cancellationToken);
    }
}