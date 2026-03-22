using EduTests.Database.Entities;

namespace EduTests.Database.Repositories.Interfaces;

public interface IUserRatingRepository : IRepository<UserRating, int>
{
    Task<int> GetTestRatingAsync(int id, CancellationToken cancellationToken);
    Task<Dictionary<int, int>> GetTestRatingsAsync(IEnumerable<int> ids, CancellationToken cancellationToken);
    Task<UserRating?> GetUsersRatingAsync(int testId, int userId, CancellationToken cancellationToken);
}