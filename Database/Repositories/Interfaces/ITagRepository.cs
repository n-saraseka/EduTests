using EduTests.Database.Entities;

namespace EduTests.Database.Repositories.Interfaces;

public interface ITagRepository : IRepository<Tag, int>
{
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Tag>> GetByNameBulkAsync(IEnumerable<string> names, CancellationToken cancellationToken = default);
}