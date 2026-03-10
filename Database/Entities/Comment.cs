namespace EduTests.Database.Entities;

public class Comment
{
    public int Id { get; set; }
    public required User Commenter { get; set; }
    public User? UserProfile { get; set; }
    public Test? Test { get; set; }
    public required string Content { get; set; }
    public required DateTime DateTime { get; set; }
}