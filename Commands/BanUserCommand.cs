using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands;

public class BanUserCommand
{
    [Required]
    public int UserId { get; set; }
    [Required]
    public string Reason { get; set; }
    public DateTime? UnbanDate { get; set; }
}