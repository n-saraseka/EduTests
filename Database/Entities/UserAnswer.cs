namespace EduTests.Database.Entities;

public class UserAnswer
{
    public int Id { get; set; }
    public TestCompletion Completion { get; set; }
    public Question Question { get; set; }
    public required QuestionData Answers { get; set; }
}