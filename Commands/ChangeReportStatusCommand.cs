using EduTests.Database.Enums;

namespace EduTests.Commands;

public class ChangeReportStatusCommand
{
    public int ReportId { get; set; }
    public ReportStatus Status { get; set; }
}