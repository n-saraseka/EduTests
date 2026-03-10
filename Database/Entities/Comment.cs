namespace EduTests.Database.Entities;

public class Comment
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public required DateTime DateTime { get; set; }
}