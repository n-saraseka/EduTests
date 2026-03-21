using EduTests.Database.Entities;
using EduTests.Database.Enums;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class TestRepository(DatabaseContext db) : BaseRepository<Test, int>(db), ITestRepository
{
    /// <summary>
    /// Search <see cref="Test"/>s that have matching text in their name, description, or tags
    /// </summary>
    /// <param name="text">Text to match</param>
    /// <returns>List of all <see cref="Test"/>s matching the text</returns>
    public IQueryable<Test> Search(string text) =>
        Set.Where(t => (t.Name.Contains(text)
                        || t.Description != null && t.Description.Contains(text)
                        || t.Tags.Any(tag => tag.Name.Contains(text)))
                       && t.AccessType == AccessType.Public);
    
    /// <summary>
    /// Get all <see cref="Test"/>s that have matching <see cref="Tag"/> name
    /// </summary>
    /// <param name="name">Name to match</param>
    /// <returns>An <see cref="IQueryable"/> of all <see cref="Test"/>s with the <see cref="Tag"/></returns>
    public IQueryable<Test> GetAllByTag(string name) =>
        Set.Where(t => t.Tags.Any(tag => tag.Name == name));

    /// <summary>
    /// Get a <see cref="Test"/> by its ID, including tags
    /// </summary>
    /// <param name="id"><see cref="Test"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>An <see cref="IQueryable"/> with the <see cref="Test"/> and its loaded <see cref="Tag"/>s</returns>
    public Task<Test?> GetByIdWithTagsAsync(int id, CancellationToken cancellationToken = default) =>
        Set.Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == id);
}