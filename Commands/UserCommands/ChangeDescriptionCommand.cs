using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands.UserCommands;

public class ChangeDescriptionCommand
{
    [Required]
    public string Description { get; set; }
}