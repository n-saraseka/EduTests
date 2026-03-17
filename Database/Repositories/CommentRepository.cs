using EduTests.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class CommentRepository(DatabaseContext db) : BaseRepository<Comment, int>(db)
{
    /// <summary>
    /// Get all <see cref="Comment"/>s from profile
    /// </summary>
    /// <param name="id"><see cref="User"/> ID</param>
    /// <returns>An <see cref="IQueryable"/> containing all comments on that <see cref="User"/>'s profile</returns>
    public IQueryable<Comment> GetProfileComments(int id) =>
        Set.Where(c => c.UserProfileId == id).OrderByDescending(c => c.DateTime);
    
    /// <summary>
    /// Get all <see cref="Comment"/>s on a <see cref="Test"/>
    /// </summary>
    /// <param name="id"><see cref="Test"/> ID</param>
    /// <returns>An <see cref="IQueryable"/> containing all comments on that <see cref="Test"/></returns>
    public IQueryable<Comment> GetTestComments(int id) =>
        Set.Where(c => c.TestId == id).OrderByDescending(c => c.DateTime);
    
    /// <summary>
    /// Get <see cref="Comment"/>s page
    /// </summary>
    /// <param name="number">Page number to get <see cref="Comment"/>s from</param>
    /// <param name="count">Count of <see cref="Comment"/>s per page</param>
    /// <param name="tests">An <see cref="IQueryable"/> containing <see cref="Comment"/> data</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>A list containing all <see cref="Comment"/>s on this page</returns>
    /// <exception cref="TaskCanceledException">If the <see cref="CancellationToken"/> is canceled</exception>
    public Task<List<Comment>> GetPageAsync(int number, int count, IQueryable<Comment> tests, CancellationToken cancellationToken) =>
        tests.Skip((number - 1) * count).Take(count).ToListAsync(cancellationToken);
}