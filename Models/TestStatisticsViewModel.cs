using EduTests.ApiObjects;

namespace EduTests.Models;

public class TestStatisticsViewModel
{
    public ApiTest Test { get; set; }
    public List<ApiCompletion> Completions { get; set; } = new();
    public List<ApiQuestion> Questions { get; set; } = new();
    public int PageSize { get; set; }
    public int Pages { get; set; }
    public ApiCompletionStats CompletionStats { get; set; } = new();
    public required List<DateTime> Versions { get; set; }
}