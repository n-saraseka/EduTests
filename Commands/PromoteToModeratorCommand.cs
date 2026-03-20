using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands;

public class PromoteToModeratorCommand
{
    [Required]
    public int UserId { get; set; }
}