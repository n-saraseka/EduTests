using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class TagRepository(DatabaseContext db) : BaseRepository<Tag, int>(db), ITagRepository
{
    /// <summary>
    /// Get a <see cref="Tag"/> by its name
    /// </summary>
    /// <param name="name">The <see cref="Tag"/> name</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="Tag"/> or null</returns>
    public Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return Set.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    /// <summary>
    /// Get <see cref="Tag"/>s by their names
    /// </summary>
    /// <param name="names">An <see cref="IEnumerable{string}"/> containing the tag names</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>List of matching <see cref="Tag"/> entities</returns>
    public Task<List<Tag>> GetByNameBulkAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
    {
        return Set.Where(t => names.Contains(t.Name)).ToListAsync(cancellationToken);
    }

    public IQueryable<Tag> GetPopularTags() =>
        Set.Include(t => t.Tests)
            .AsSplitQuery()
            .OrderByDescending(t => t.Tests.Count);
}