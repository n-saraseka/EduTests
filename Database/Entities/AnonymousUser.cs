namespace EduTests.Database.Entities;

public class AnonymousUser : IAuditable
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}