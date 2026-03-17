using EduTests.Database.Enums;
using EduTests.Database.Entities.Questions;

namespace EduTests.Database.Entities;

public class Question : IEntity<int>
{
    public int Id { get; set; }
    public required Test Test { get; set; }
    public required int TestId { get; set; }
    public required int OrderIndex { get; set; }
    public required QuestionType Type { get; set; }
    public required string Description { get; set; }
    public required QuestionData Data { get; set; }
    public QuestionData? CorrectData { get; set; }
}