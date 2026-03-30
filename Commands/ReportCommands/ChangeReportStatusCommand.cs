using System.ComponentModel.DataAnnotations;
using EduTests.Database.Enums;

namespace EduTests.Commands.ReportCommands;

public class ChangeReportStatusCommand
{
    [Required]
    public int ReportStatus { get; set; }
}