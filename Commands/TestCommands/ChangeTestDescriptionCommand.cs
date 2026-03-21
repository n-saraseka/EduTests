using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands.TestCommands;

public class ChangeTestDetailsCommand
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? AttemptLimit { get; set; }
    
}