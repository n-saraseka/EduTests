namespace EduTests.Database.Entities;

public class TestCompletion
{
    public int Id { get; set; }
    public required int CorrectAnswers { get; set; }
    public required float CompletionPercentage { get; set; }
    public required TimeSpan CompletionTime { get; set; }
    public required DateTime CompletedAt { get; set; }
}