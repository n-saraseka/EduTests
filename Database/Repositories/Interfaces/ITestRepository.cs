using EduTests.Database.Entities;

namespace EduTests.Database.Repositories.Interfaces;

public interface ITestRepository : IRepository<Test, int>
{
    IQueryable<Test> Search(string text);
    IQueryable<Test> GetAllByTag(string name);
    Task<List<Test>> GetPageAsync(int number, int count, IQueryable<Test> tests, CancellationToken cancellationToken);
}