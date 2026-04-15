using EduTests.ApiObjects;
using EduTests.Database.Repositories.Interfaces;

namespace EduTests.Services;

public class TestStatsService(IUserRatingRepository ratingRepository,
    ITestCompletionRepository testCompletionRepository): ITestStatsService
{
    public async Task<ApiTest> GetTestStatsAsync(ApiTest test, CancellationToken token = default)
    {
        test.Rating = await ratingRepository.GetTestRatingAsync(test.Id, token);
        test.CompletionCount = await testCompletionRepository.GetTestCompletionCountAsync(test.Id, token);
        return test;
    }

    public async Task<IEnumerable<ApiTest>> GetTestsStatsAsync(IEnumerable<ApiTest> tests, CancellationToken token = default)
    {
        var ids = tests.Select(t => t.Id);
        var ratings = await ratingRepository.GetTestRatingsAsync(ids, token);
        var counts = await testCompletionRepository.GetTestCompletionCountsAsync(ids, token);
        return tests.Select(t =>
        {
            t.Rating = ratings[t.Id];
            t.CompletionCount = counts[t.Id];
            return t;
        });
    }
}