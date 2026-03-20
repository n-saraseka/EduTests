using EduTests.Database.Enums;

namespace EduTests.ApiObjects;

public class ApiReport
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public int? TestId { get; set; }
    public int? CommentId { get; set; }
    public required string ReportText { get; set; }
    public DateTime DateReported { get; set; }
    public ReportStatus ReportStatus { get; set; }
}