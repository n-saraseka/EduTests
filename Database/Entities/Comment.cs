namespace EduTests.Database.Entities;

public class Comment : IEntity<int>, IAuditable 
{
    public int Id { get; set; }
    public required User Commenter { get; set; }
    public required int CommenterId { get; set; }
    public User? UserProfile { get; set; }
    public int? UserProfileId { get; set; }
    public Test? Test { get; set; }
    public int? TestId { get; set; }
    public required string Content { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}