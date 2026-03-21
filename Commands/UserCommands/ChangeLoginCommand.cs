using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands.UserCommands;

public class ChangeLoginCommand
{
    [Required]
    public string Login { get; set; }
    
    [Required]
    public string Password { get; set; }
}