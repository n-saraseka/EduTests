using System.ComponentModel.DataAnnotations;
using EduTests.Database.Entities;

namespace EduTests.Commands.TestCommands;

public class AnswerTestCommand
{
    [Required]
    public int QuestionId { get; set; }
    [Required]
    public QuestionData Answer { get; set; }
}