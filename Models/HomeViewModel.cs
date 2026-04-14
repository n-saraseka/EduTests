using EduTests.ApiObjects;

namespace EduTests.Models;

public class HomeViewModel
{
    public List<ApiTest> Tests { get; set; } = new();
    public List<ApiTag> PopularTags { get; set; } = new();
    public int Page { get; set; }
    public int Pages { get; set; }
    public string? TagName { get; set; }
}