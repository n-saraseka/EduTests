namespace EduTests.Database.Entities;

public class Tag : IEntity<int>
{
    public int Id { get; set; }
    public required string Name { get; set; }
}