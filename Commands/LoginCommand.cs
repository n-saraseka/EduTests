using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands;

public class LoginCommand
{
    public required string Login { get; set; }
    
    [MinLength(8)]
    public required string Password { get; set; }
}