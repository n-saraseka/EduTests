using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands.CommentCommands;

public class CreateCommentCommand
{
    [Required]
    public string Content {get; set;}
}