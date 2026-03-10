namespace EduTests.Database.Entities;

public class BannedUser
{
    public int Id { get; set; }
    public required User UserBanned { get; set; }
    public required User BannedBy { get; set; }
    public required string BanReason { get; set; }
    public required DateTime DateBanned { get; set; }
    public DateTime? DateUnbanned { get; set; }
}