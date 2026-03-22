using System.ComponentModel.DataAnnotations;

namespace EduTests.ApiObjects;

public class ApiTest
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public required List<string> Tags { get; set; }
    public required int Rating { get; set; }
    public required int CompletionCount { get; set; }
    public int? AttemptLimit { get; set; }
    public TimeSpan? TimeLimit { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}