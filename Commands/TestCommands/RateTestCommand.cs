using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands.TestCommands;

public class RateTestCommand
{
    [Required]
    public bool IsPositive { get; set; }
}