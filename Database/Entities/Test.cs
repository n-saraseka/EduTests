using EduTests.Database.Enums;

namespace EduTests.Database.Entities;

public class Test
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public AccessType AccessType { get; set; }
    public string? Password { get; set; }
    public int? AttemptLimit { get; set; }
    public TimeSpan? TimeLimit { get; set; }
}