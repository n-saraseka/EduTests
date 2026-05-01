using EduTests.ApiObjects;

namespace EduTests.Models;

public class ResultDetailsViewModel
{
    public required ApiCompletion Completion { get; set; }
    public string? ResultString { get; set; }
    public required List<ApiAnswer> Answers { get; set; }
    public required List<ApiQuestion> Questions { get; set; }
    public required int Pages { get; set; }
    public required int PageSize { get; set; }
}