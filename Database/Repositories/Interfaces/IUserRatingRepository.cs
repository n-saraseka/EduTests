namespace EduTests.Database.Repositories.Interfaces;

public interface IUserRatingRepository
{
    Task<int> ComputeTestRatingAsync(int testId, CancellationToken cancellationToken);
}