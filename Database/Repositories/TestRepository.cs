using EduTests.Database.Entities;
using EduTests.Database.Enums;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class TestRepository(DatabaseContext db) : BaseRepository<Test, int>(db)
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
    /// Get <see cref="Test"/>s page
    /// </summary>
    /// <param name="number">Page number to get <see cref="Test"/>s from</param>
    /// <param name="count">Count of <see cref="Test"/>s per page</param>
    /// <param name="tests">An <see cref="IQueryable"/> containing <see cref="Test"/> data</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>A list containing all <see cref="Test"/>s on this page</returns>
    /// <exception cref="TaskCanceledException">If the <see cref="CancellationToken"/> is canceled</exception>
    public Task<List<Test>> GetPageAsync(int number, int count, IQueryable<Test> tests, CancellationToken cancellationToken) =>
        tests.Skip((number - 1) * count).Take(count).ToListAsync(cancellationToken);
}