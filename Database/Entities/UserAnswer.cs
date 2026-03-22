namespace EduTests.Database.Entities;

public class UserAnswer : IEntity<int>
{
    public int Id { get; set; }
    public TestCompletion Completion { get; set; }
    public required int TestCompletionId { get; set; }
    public Question Question { get; set; }
    public required int QuestionId { get; set; }
    public required QuestionData Answers { get; set; }
}