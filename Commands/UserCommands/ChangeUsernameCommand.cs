using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands.UserCommands;

public class ChangeUsernameCommand
{
    [Required]
    public string Username { get; set; }
}