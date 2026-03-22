namespace EduTests.Database.Entities;

public class TestCompletion : IEntity<int>
{
    public int Id { get; set; }
    public User User { get; set; }
    public int? UserId { get; set; }
    public AnonymousUser AnonymousUser { get; set; }
    public Guid? AnonymousUserId { get; set; }
    public Test Test { get; set; }
    public required int TestId { get; set; }
    public required DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}