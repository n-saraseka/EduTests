using EduTests.Database.Entities;

namespace EduTests.Database.Repositories;

public class UserAnswerRepository(DatabaseContext db) : BaseRepository<UserAnswer, int>(db)
{
    public override void Create(UserAnswer item)
    {
        item.Answers.ValidateAnswer();
        base.Create(item);
    }

    public override void CreateBulk(IEnumerable<UserAnswer> items)
    {
        foreach (var item in items)
            item.Answers.ValidateAnswer();
        base.CreateBulk(items);
    }

    public override void Update(UserAnswer item)
    {
        item.Answers.ValidateAnswer();
        base.Update(item);
    }

    public override void UpdateBulk(IEnumerable<UserAnswer> items)
    {
        foreach (var item in items)
            item.Answers.ValidateAnswer();
        base.UpdateBulk(items);
    }
}