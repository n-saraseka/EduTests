using System.ComponentModel.DataAnnotations;
using EduTests.ApiObjects;

namespace EduTests.Commands.TestCommands;

public class CreateOrUpdateTestCommand
{
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }
    public List<string> Tags { get; set; }
    public int? AttemptLimit { get; set; }
    public TimeSpan? TimeLimit { get; set; }
    public string? Password { get; set; }
    [Required]
    public List<ApiQuestion> Questions { get; set; }
    [Required]
    public List<ApiTestResult> Results { get; set; }
}