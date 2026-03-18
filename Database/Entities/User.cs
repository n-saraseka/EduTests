using EduTests.Database.Enums;

namespace EduTests.Database.Entities;

public class User : IEntity<int>
{
    public int Id { get; set; }
    public required string Login { get; set; }
    public required string PasswordHash { get; set; }
    public required string Username { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Description { get; set; }
    public DateTime RegistrationDate { get; set; }
    public UserGroup Group { get; set; } = UserGroup.User;
}