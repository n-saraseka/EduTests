using System.ComponentModel.DataAnnotations;
using EduTests.Database.Enums;

namespace EduTests.Commands.ReportCommands;

public class ChangeReportStatusCommand
{
    [Required]
    public ReportStatus ReportStatus { get; set; }
}