using EduTests.Database.Enums;

namespace EduTests.Database.Entities;

public class Report
{
    public int Id { get; set; }
    public required string Text { get; set; }
    public required DateTime DateTime { get; set; }
    public required ReportStatus ReportStatus { get; set; }
}