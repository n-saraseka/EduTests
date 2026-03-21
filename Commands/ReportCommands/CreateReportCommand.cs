using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands.ReportCommands;

public class CreateReportCommand
{
    [Required]
    public EntityType EntityType { get; set; }
    [Required]
    public int EntityId { get; set; }
    [Required]
    public string Text { get; set; }
}