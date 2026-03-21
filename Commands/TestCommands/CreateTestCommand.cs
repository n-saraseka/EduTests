using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands.TestCommands;

public class CreateTestCommand
{
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }
}