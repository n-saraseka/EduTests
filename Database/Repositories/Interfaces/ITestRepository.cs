using EduTests.Database.Entities;

namespace EduTests.Database.Repositories.Interfaces;

public interface ITestRepository : IRepository<Test, int>
{
    IQueryable<Test> Search(string text);
    IQueryable<Test> GetAllByTag(string name);
    IQueryable<Test> GetAllWithTags();
    Task<Test?> GetByIdWithTagsAsync(int id, CancellationToken cancellationToken);
    Task<Test?> GetByIdWithExtendedDataAsync(int id, CancellationToken cancellationToken);
    IQueryable<Test> GetByUserId(int userId);
}