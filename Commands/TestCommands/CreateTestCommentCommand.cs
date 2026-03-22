using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands.TestCommands;

public class CreateTestCommentCommand
{
    [Required]
    public string Content {get; set;}
}