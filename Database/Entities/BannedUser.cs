namespace EduTests.Database.Entities;

public class BannedUser : IEntity<int>
{
    public int Id { get; set; }
    public User UserBanned { get; set; }
    public required int UserBannedId { get; set; }
    public User BannedBy { get; set; }
    public required int BannedById { get; set; }
    public required string BanReason { get; set; }
    public required DateTime DateBanned { get; set; }
    public DateTime? DateUnbanned { get; set; }
}