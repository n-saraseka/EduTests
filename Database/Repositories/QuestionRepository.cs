using EduTests.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class QuestionRepository(DatabaseContext db) : BaseRepository<Question, int>(db)
{
    public override void Create(Question item)
    {
        item.Data.Validate(item.CorrectData, item.Type);
        base.Create(item);
    }
}