using EduTests.ApiObjects;

namespace EduTests.Models;

public class ProfileViewModel
{
    public required ApiUser User { get; set; }
    public List<ApiComment> Comments { get; set; } = new();
    public required int CommentPages { get; set; }
    public required int CommentsPerPage { get; set; }
    public int? CurrentUserId { get; set; }
    public string? CurrentUserGroup { get; set; }
    public bool IsBanned { get; set; } = false;
    public bool IsCurrentBanned { get; set; } = false;
    public List<ApiTest> Tests { get; set; } = new();
    public int TestPageSize { get; set; }
    public int TestPages { get; set; }
}