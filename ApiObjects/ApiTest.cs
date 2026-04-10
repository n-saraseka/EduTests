namespace EduTests.ApiObjects;

public class ApiTest
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public List<string>? Tags { get; set; }
    public List<ApiQuestion>? Questions { get; set; }
    public Dictionary<float, string>? Results { get; set; }
    public int? Rating { get; set; }
    public int? CompletionCount { get; set; }
    public int? AttemptLimit { get; set; }
    public TimeSpan? TimeLimit { get; set; }
    public string? DefaultResult { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}