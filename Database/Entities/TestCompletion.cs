namespace EduTests.Database.Entities;

public class TestCompletion : IEntity<int>
{
    public int Id { get; set; }
    public User User { get; set; }
    public required int UserId { get; set; }
    public Test Test { get; set; }
    public required int TestId { get; set; }
    public required int CorrectAnswers { get; set; }
    public required float CompletionPercentage { get; set; }
    public required TimeSpan CompletionTime { get; set; }
    public required DateTime CompletedAt { get; set; }
}