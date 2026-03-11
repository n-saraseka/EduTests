namespace EduTests.Database.Entities;

public class Tag
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Test> Tests { get; set; } = new();
}