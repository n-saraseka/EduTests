using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class QuestionRepository(DatabaseContext db) : BaseRepository<Question, int>(db), IQuestionRepository
{
    public Task<List<Question>> GetByTestIdAsync(int testId, CancellationToken cancellationToken)
    {
        return Set.Where(q => q.TestId == testId).ToListAsync(cancellationToken);
    }
}