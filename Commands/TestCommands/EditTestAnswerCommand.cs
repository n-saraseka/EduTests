using System.ComponentModel.DataAnnotations;
using EduTests.Database.Entities;

namespace EduTests.Commands.TestCommands;

public class EditTestAnswerCommand
{
    [Required]
    public QuestionData NewAnswer { get; set; }
}