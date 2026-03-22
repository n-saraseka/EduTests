using System.ComponentModel.DataAnnotations;
using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.ApiObjects;

public class ApiQuestion
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public required int OrderIndex { get; set; }
    public required QuestionType Type { get; set; }
    public required string Description { get; set; }
    public required QuestionData Data { get; set; }
    public required QuestionData CorrectData { get; set; }
}