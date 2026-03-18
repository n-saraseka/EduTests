using EduTests.Database.Entities;

namespace EduTests.Database.Repositories.Interfaces;

public interface ICommentRepository : IRepository<Comment, int>
{
    IQueryable<Comment> GetProfileComments(int id);
    IQueryable<Comment> GetTestComments(int id);
    Task<List<Comment>> GetPageAsync(int number, int count, IQueryable<Comment> tests,
        CancellationToken cancellationToken);
}