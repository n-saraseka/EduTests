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
        Set.Where(c => c.UserProfileId == id).OrderByDescending(c => c.CreatedAt);
    
    /// <summary>
    /// Get all <see cref="Comment"/>s on a <see cref="Test"/>
    /// </summary>
    /// <param name="id"><see cref="Test"/> ID</param>
    /// <returns>An <see cref="IQueryable"/> containing all comments on that <see cref="Test"/></returns>
    public IQueryable<Comment> GetTestComments(int id) =>
        Set.Where(c => c.TestId == id).OrderByDescending(c => c.CreatedAt);
}