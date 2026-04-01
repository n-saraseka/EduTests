using System.ComponentModel.DataAnnotations;

namespace EduTests.ApiObjects;

public class ApiBan
{
    public required int Id { get; set; }
    public required int BannedUserId { get; set; }
    public ApiUser? BannedUser  { get; set; }
    public required int BannedByUserId { get; set; }
    public ApiUser? BannedByUser { get; set; }
    public required string BanReason { get; set; }
    public required DateTime BanDate { get; set; }
    public DateTime? UnbanDate { get; set; }
}