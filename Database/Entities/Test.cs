using EduTests.Database.Enums;

namespace EduTests.Database.Entities;

public class Test : IEntity<int>, IAuditable
{
    public int Id { get; set; }
    public User User { get; set; }
    public required int UserId { get; set; }
    public List<Tag> Tags { get; set; } = new();
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public AccessType AccessType { get; set; }
    public string? Password { get; set; }
    public int? AttemptLimit { get; set; }
    public TimeSpan? TimeLimit { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}