namespace EduTests.Database.Entities;

public class UserAnswer
{
    public int Id { get; set; }
    public required TestCompletion Completion { get; set; }
    public required Question Question { get; set; }
    public required QuestionData Answers { get; set; }
}