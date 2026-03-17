using EduTests.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class QuestionRepository(DatabaseContext db) : BaseRepository<Question, int>(db)
{
    public override void Create(Question item)
    {
        item.Data.ValidateQuestion(item.CorrectData, item.Type);
        base.Create(item);
    }

    public override void CreateBulk(IEnumerable<Question> items)
    {
        foreach (var item in items)
            item.Data.ValidateQuestion(item.CorrectData, item.Type);
        base.CreateBulk(items);
    }

    public override void Update(Question item)
    {
        item.Data.ValidateQuestion(item.CorrectData, item.Type);
        base.Update(item);
    }

    public override void UpdateBulk(IEnumerable<Question> items)
    {
        foreach (var item in items)
            item.Data.ValidateQuestion(item.CorrectData, item.Type);
        base.UpdateBulk(items);
    }
}