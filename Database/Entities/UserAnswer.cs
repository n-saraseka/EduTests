namespace EduTests.Database.Entities;

public class UserAnswer
{
    public int Id { get; set; }
    public required QuestionData Answers { get; set; }
}