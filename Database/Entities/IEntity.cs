namespace EduTests.Database.Entities;

public interface IEntity<TKey>
{
    TKey Id { get; }
}