using EduTests.Database.Enums;

namespace EduTests.ApiObjects;

public class ApiUser
{
    public int Id  { get; set; }
    public required string Username { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Description { get; set; }
    public DateTime RegistrationDate { get; set; }
    public UserGroup Group { get; set; } = UserGroup.User;
}