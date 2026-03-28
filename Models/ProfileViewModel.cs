using EduTests.ApiObjects;

namespace EduTests.Models;

public class ProfileViewModel
{
    public required ApiUser User { get; set; }
    public required List<ApiComment> Comments { get; set; }
    public required int CommentPages { get; set; }
    public required int CommentsPerPage { get; set; }
    public required bool IsAuthorized { get; set; }
}