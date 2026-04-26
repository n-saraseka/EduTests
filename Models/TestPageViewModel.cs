using EduTests.ApiObjects;

namespace EduTests.Models;

public class TestPageViewModel
{
    public required ApiTest Test { get; set; }
    public List<ApiComment> Comments { get; set; } = new();
    public required int CommentPages { get; set; } = 1;
    public required int CommentsPerPage { get; set; }
    public int? CurrentUserId { get; set; }
    public string? CurrentUserGroup { get; set; }
    public bool IsCurrentBanned { get; set; } = false;
    public required bool CanStartTest { get; set; } = true;
}