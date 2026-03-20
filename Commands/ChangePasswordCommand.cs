using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands;

public class ChangePasswordCommand
{
    [Required]
    public string OldPassword { get; set; }
    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; }
}