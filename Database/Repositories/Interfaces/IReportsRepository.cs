using EduTests.Database.Entities;

namespace EduTests.Database.Repositories.Interfaces;

public interface IReportsRepository : IRepository<Report, int>
{
    Task<Report?> GetByTestAndReporterIdAsync(int id, int reporterId, CancellationToken cancellationToken);
    Task<Report?> GetByUserAndReporterIdAsync(int id, int reporterId, CancellationToken cancellationToken);
    Task<Report?> GetByCommentAndReporterIdAsync(int id, int reporterId, CancellationToken cancellationToken);
    Task<Report?> GetByTestAndAnonReporterIdAsync(int id, Guid reporterId, CancellationToken cancellationToken);
    Task<Report?> GetByUserAndAnonReporterIdAsync(int id, Guid reporterId, CancellationToken cancellationToken);
    Task<Report?> GetByCommentAndAnonReporterIdAsync(int id, Guid reporterId, CancellationToken cancellationToken);
}