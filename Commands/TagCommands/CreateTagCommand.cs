using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands.TagCommands;

public class CreateTagCommand
{
    [Required]
    public string Name { get; set; }
}