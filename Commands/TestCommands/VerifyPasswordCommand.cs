using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands.TestCommands;

public class VerifyPasswordCommand
{
    [Required]
    public string Password {get; set;}
}