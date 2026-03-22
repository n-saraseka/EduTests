using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class QuestionRepository(DatabaseContext db) : BaseRepository<Question, int>(db), IQuestionRepository
{
    /// <summary>
    /// Get all <see cref="Question"/>s from <see cref="Test"/>
    /// </summary>
    /// <param name="testId"><see cref="Test"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>List of <see cref="Question"/>s</returns>
    public Task<List<Question>> GetByTestIdAsync(int testId, CancellationToken cancellationToken)
    {
        return Set.Where(q => q.TestId == testId).ToListAsync(cancellationToken);
    }
}