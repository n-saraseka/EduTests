using System.ComponentModel.DataAnnotations;

namespace EduTests.ApiObjects;

public class ApiBan
{
    [Required]
    public int Id { get; set; }
    [Required]
    public int BannedUserId  { get; set; }
    [Required]
    public int BannedByUserId { get; set; }
    [Required]
    public string BanReason { get; set; }
    [Required]
    public DateTime BanDate { get; set; }
    public DateTime? UnbanDate { get; set; }
}