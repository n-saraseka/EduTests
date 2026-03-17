using EduTests.Database.Enums;

namespace EduTests.Database.Entities;

public class Report : IEntity<int>
{
    public int Id { get; set; }
    public User? User { get; set; }
    public int? UserId { get; set; }
    public Test? Test { get; set; }
    public int? TestId { get; set; }
    public Comment? Comment { get; set; }
    public int? CommentId { get; set; }
    public required string Text { get; set; }
    public required DateTime DateTime { get; set; }
    public required ReportStatus ReportStatus { get; set; }
}