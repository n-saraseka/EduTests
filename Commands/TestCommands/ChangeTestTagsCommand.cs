using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands.TestCommands;

public class ChangeTestTagsCommand
{
    [Required]
    public List<string> Tags { get; set; }
}