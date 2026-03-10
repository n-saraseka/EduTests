namespace EduTests.Database.Entities;

public class TestResult
{
    public int Id { get; set; }
    public required float PercentageThreshold { get; set; }
    public required string Result { get; set; }
}