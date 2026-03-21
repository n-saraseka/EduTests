using System.ComponentModel.DataAnnotations;
using EduTests.Database.Entities;
using EduTests.Database.Enums;

namespace EduTests.ApiObjects;

public class ApiQuestion
{
    public int Id { get; set; }
    public int TestId { get; set; }
    [Required]
    public int OrderIndex { get; set; }
    [Required]
    public QuestionType Type { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public QuestionData Data { get; set; }
    [Required]
    public QuestionData CorrectData { get; set; }
}