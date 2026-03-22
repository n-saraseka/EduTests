using EduTests.Database.Entities;

namespace EduTests.ApiObjects;

public class ApiAnswer
{
    public int Id { get; set; }
    public required int TestCompletionId { get; set; }
    public required int QuestionId { get; set; }
    public required QuestionData Answer { get; set; }
}