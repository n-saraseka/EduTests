namespace EduTests.Database.Entities;

public class TestResult : IEntity<int>
{
    public int Id { get; set; }
    public Test Test { get; set; }
    public required int TestId { get; set; }
    public required float PercentageThreshold { get; set; }
    public required string Result { get; set; }
}