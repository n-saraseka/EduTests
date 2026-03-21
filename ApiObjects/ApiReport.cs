using EduTests.Commands.ReportCommands;
using EduTests.Database.Enums;

namespace EduTests.ApiObjects;

public class ApiReport
{
    public required int Id { get; set; }
    public required EntityType EntityType { get; set; }
    public required int EntityId { get; set; }
    public required string ReportText { get; set; }
    public DateTime DateReported { get; set; }
    public ReportStatus ReportStatus { get; set; }
}