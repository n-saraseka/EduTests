using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands.AuthCommands;

public class LoginCommand
{
    [Required]
    public string Login { get; set; }
    
    [Required]
    [MinLength(8)]
    public string Password { get; set; }
}