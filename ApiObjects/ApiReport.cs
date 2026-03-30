using EduTests.Commands.ReportCommands;
using EduTests.Database.Enums;

namespace EduTests.ApiObjects;

public class ApiReport
{
    public required int Id { get; set; }
    public ApiTest? ReportedTest { get; set; }
    public ApiUser? ReportedUser { get; set; }
    public ApiComment? ReportedComment { get; set; }
    public required string ReportText { get; set; }
    public DateTime DateReported { get; set; }
    public ReportStatus ReportStatus { get; set; }
}