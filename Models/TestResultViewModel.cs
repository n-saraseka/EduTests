using EduTests.ApiObjects;

namespace EduTests.Models;

public class TestResultViewModel
{
    public required ApiCompletion Completion { get; set; }
    public List<ApiComment> Comments { get; set; } = new();
    public required int CommentPages { get; set; }
    public required int CommentsPerPage { get; set; }
    public int? CurrentUserId { get; set; }
    public string? CurrentUserGroup { get; set; }
    public bool IsCurrentBanned { get; set; } = false;
    public ApiRating? CurrentRating { get; set; }
}