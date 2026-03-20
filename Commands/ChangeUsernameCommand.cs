using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands;

public class ChangeUsernameCommand
{
    [Required]
    public string Username { get; set; }
}