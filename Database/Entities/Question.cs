using EduTests.Database.Enums;

namespace EduTests.Database.Entities;

public class Question
{
    public int Id { get; set; }
    public required int OrderIndex { get; set; }
    public required QuestionType Type { get; set; }
    public required string Description { get; set; }
    public required QuestionData Data { get; set; }
    public required QuestionData CorrectData { get; set; }
}