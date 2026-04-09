using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands.UserCommands;

public class ChangeDescriptionCommand
{
    public string? Description { get; set; }
}