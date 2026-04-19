namespace EduTests.ApiObjects;

public class ApiCompletion
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public ApiUser? User { get; set; }
    public required int TestId { get; set; }
    public ApiTest? Test { get; set; }
    public int? CorrectAnswers { get; set; }
    public double? CompletionPercentage { get; set; }
    public required DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}