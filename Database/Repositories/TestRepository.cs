using EduTests.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class TestRepository(DatabaseContext db) : BaseRepository<Test, int>(db)
{
    /// <summary>
    /// Search <see cref="Test"/>s that have matching text in their name or description
    /// </summary>
    /// <param name="text">Text to match</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>List of all <see cref="Test"/>s matching the text</returns>
    /// <exception cref="TaskCanceledException">If the <see cref="CancellationToken"/> is canceled</exception>
    public Task<List<Test>> SearchAsync(string text, CancellationToken cancellationToken) =>
        Set.Where(t => t.Name.Contains(text) 
                       || t.Description!=null && t.Description.Contains(text)
                       || t.Tags.Any() && t.Tags.FirstOrDefault(tag => tag.Name.Contains(text)) != null)
            .ToListAsync(cancellationToken);
    
    /// <summary>
    /// Get all <see cref="Test"/>s that have matching <see cref="Tag"/> name
    /// </summary>
    /// <param name="name">Name to match</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>List of all <see cref="Test"/>s with the <see cref="Tag"/></returns>
    /// <exception cref="TaskCanceledException">If the <see cref="CancellationToken"/> is canceled</exception>
    public Task<List<Test>> GetAllByTagAsync(string name, CancellationToken cancellationToken) =>
        Set.Where(t => t.Tags.Any() && t.Tags.FirstOrDefault(tag => tag.Name == name) != null)
            .ToListAsync(cancellationToken);
}