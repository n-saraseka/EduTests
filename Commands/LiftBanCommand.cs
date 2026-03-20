using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands;

public class LiftBanCommand
{
    [Required]
    public int BanId { get; set; }
}