using EduTests.ApiObjects;

namespace EduTests.Services;

public interface ITestStatsService
{
    Task<ApiTest> GetTestStatsAsync(ApiTest test, CancellationToken token);
    Task<IEnumerable<ApiTest>> GetTestsStatsAsync(IEnumerable<ApiTest> tests, CancellationToken token);
}