using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands.ReportCommands;

public class ChangeReportStatusCommand
{
    [Required]
    public int ReportStatus { get; set; }
}