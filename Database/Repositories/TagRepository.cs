using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class TagRepository(DatabaseContext db) : BaseRepository<Tag, int>(db), ITagRepository
{
    public Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return Set.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    public Task<List<Tag>> GetByNameBulkAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
    {
        return Set.Where(t => names.Contains(t.Name)).ToListAsync(cancellationToken);
    }
}