using EduTests.ApiObjects;

namespace EduTests.Models;

public class TestPlaythroughViewModel
{
    public ApiTest Test { get; set; }
    public List<ApiQuestion> Questions { get; set; } = new();
    public List<ApiAnswer> Answers { get; set; } = new();
    public int LastUnansweredQuestion { get; set; } = 1;
}