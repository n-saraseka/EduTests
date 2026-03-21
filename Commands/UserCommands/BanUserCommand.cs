using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands.UserCommands;

public class BanUserCommand
{
    [Required]
    public string Reason { get; set; }
    public DateTime? UnbanDate { get; set; }
}