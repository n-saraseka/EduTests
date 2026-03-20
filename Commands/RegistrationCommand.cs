using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands;

public class RegistrationCommand
{
    [Required]
    public string Login { get; set; }
    
    [Required]
    [MinLength(8)]
    public string Password { get; set; }
    
    [Required]
    public string Username { get; set; }
}