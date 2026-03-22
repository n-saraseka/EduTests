namespace EduTests.ApiObjects;

public class ApiComment
{
    public int Id {get; set;}
    public required int UserId {get; set;}
    public required CommentEntityType EntityType {get; set;}
    public required int EntityId {get; set;}
    public required string Content {get; set;}
    public required DateTime CreatedAt {get; set;}
    public required DateTime UpdatedAt {get; set;}
}