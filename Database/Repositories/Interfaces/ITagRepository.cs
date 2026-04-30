using EduTests.Database.Entities;

namespace EduTests.Database.Repositories.Interfaces;

public interface ITagRepository : IRepository<Tag, int>
{
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<List<Tag>> GetByNameBulkAsync(IEnumerable<string> names, CancellationToken cancellationToken);
    IQueryable<Tag> GetPopularTags();
    Task<List<Tag>> SearchAsync(string query, CancellationToken cancellationToken);
}