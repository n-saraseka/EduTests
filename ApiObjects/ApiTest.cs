using System.ComponentModel.DataAnnotations;

namespace EduTests.ApiObjects;

public class ApiTest
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    [Required]
    public List<string> Tags { get; set; }
    [Required]
    public int Rating { get; set; }
    [Required]
    public int CompletionCount { get; set; }
    public int? AttemptLimit { get; set; }
    public TimeSpan? TimeLimit { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public DateTime UpdatedAt { get; set; }
}