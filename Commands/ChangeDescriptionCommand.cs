using System.ComponentModel.DataAnnotations;

namespace EduTests.Commands;

public class ChangeDescriptionCommand
{
    [Required]
    public string Description { get; set; }
}