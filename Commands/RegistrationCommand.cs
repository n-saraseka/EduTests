using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands;

public class RegistrationCommand
{
    public required string Login { get; set; }
    
    [MinLength(8)]
    public required string Password { get; set; }
    
    public required string Username { get; set; }
}