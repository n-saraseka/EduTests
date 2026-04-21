using EduTests.ApiObjects;

namespace EduTests.Models;

public class SearchViewModel
{
    public List<ApiTest> Tests { get; set; } = new();
    public int Page { get; set; }
    public int Pages { get; set; }
    public required string SearchQuery { get; set; }
}