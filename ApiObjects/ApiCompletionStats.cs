namespace EduTests.ApiObjects;

public class ApiCompletionStats
{
    public TimeSpan? MinTime { get; set; }
    public TimeSpan? InterQuartileAverageTime { get; set; }
    public TimeSpan? MaxTime { get; set; }
    public double? MinPercentage { get; set; }
    public double? MedianPercentage { get; set; }
    public double? MaxPercentage { get; set; }
    public int CompletionCount { get; set; }
}