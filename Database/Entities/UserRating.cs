namespace EduTests.Database.Entities;

public class UserRating : IEntity<int>
{
    public int Id { get; set; }
    public User User { get; set; }
    public required int UserId { get; set; }
    public Test Test { get; set; }
    public required int TestId { get; set; }
    public required bool IsPositive { get; set; }
}