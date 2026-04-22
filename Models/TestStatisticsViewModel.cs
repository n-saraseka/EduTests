using EduTests.ApiObjects;

namespace EduTests.Models;

public class TestStatisticsViewModel
{
    public ApiTest Test { get; set; }
    public List<ApiCompletion> Completions { get; set; } = new();
    public int PageSize { get; set; }
    public int Pages { get; set; }
    public TimeSpan? MinTime { get; set; }
    public TimeSpan? InterQuartileAverageTime { get; set; }
    public TimeSpan? MaxTime { get; set; }
    public double? MinPercentage { get; set; }
    public double? MedianPercentage { get; set; }
    public double? MaxPercentage { get; set; }
}