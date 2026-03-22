using EduTests.Database.Entities;

namespace EduTests.Database.Repositories.Interfaces;

public interface IReportsRepository : IRepository<Report, int>
{
    Task<Report?> GetByTestAndReporterIdAsync(int id, int? reporterId, Guid? anonReporterId,
        CancellationToken cancellationToken);
    
    Task<Report?> GetByUserAndReporterIdAsync(int id, int? reporterId, Guid? anonReporterId,
        CancellationToken cancellationToken);

    Task<Report?> GetByCommentAndReporterIdAsync(int id, int? reporterId, Guid? anonReporterId,
        CancellationToken cancellationToken);
}