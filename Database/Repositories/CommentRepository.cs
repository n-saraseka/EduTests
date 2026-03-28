using EduTests.Database.Entities;
using Microsoft.EntityFrameworkCore;
using EduTests.Database.Repositories.Interfaces;

namespace EduTests.Database.Repositories;

public class CommentRepository(DatabaseContext db) : BaseRepository<Comment, int>(db), ICommentRepository
{
    /// <summary>
    /// Get all <see cref="Comment"/>s from profile
    /// </summary>
    /// <param name="id"><see cref="User"/> ID</param>
    /// <returns>An <see cref="IQueryable"/> containing all comments on that <see cref="User"/>'s profile</returns>
    public IQueryable<Comment> GetProfileComments(int id) =>
        Set.Where(c => c.UserProfileId == id).OrderByDescending(c => c.CreatedAt)
            .Include(c => c.Commenter)
            .AsSplitQuery();
    
    /// <summary>
    /// Get all <see cref="Comment"/>s on a <see cref="Test"/>
    /// </summary>
    /// <param name="id"><see cref="Test"/> ID</param>
    /// <returns>An <see cref="IQueryable"/> containing all comments on that <see cref="Test"/></returns>
    public IQueryable<Comment> GetTestComments(int id) =>
        Set.Where(c => c.TestId == id).OrderByDescending(c => c.CreatedAt)
        .Include(c => c.Commenter)
        .AsSplitQuery();
    
    /// <summary>
    /// Get a <see cref="Comment"/> by its ID, including the <see cref="Comment.Commenter"/> entity
    /// </summary>
    /// <param name="id">The <see cref="Comment"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns>The <see cref="Comment"/> or null</returns>
    public Task<Comment?> GetWithLoadedCommenter(int id, CancellationToken cancellationToken = default) =>
        Set.Include(c => c.Commenter)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
}