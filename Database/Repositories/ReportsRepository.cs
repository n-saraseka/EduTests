using EduTests.Database.Entities;
using EduTests.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database.Repositories;

public class ReportsRepository(DatabaseContext db) : BaseRepository<Report, int>(db), IReportsRepository
{
    /// <summary>
    /// Get the latest report of this <see cref="Test"/> by this <see cref="User"/>
    /// </summary>
    /// <param name="id"><see cref="Test"/> ID</param>
    /// <param name="reporterId"><see cref="User"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="Report"/> or null</returns>
    public Task<Report?> GetByTestAndReporterIdAsync(int id, int reporterId, CancellationToken cancellationToken) => 
        Set
            .AsQueryable()
            .OrderByDescending(r => r.DateTime)
            .FirstOrDefaultAsync(r => r.TestId == id && r.ReportingUserId == reporterId, cancellationToken);
    
    /// <summary>
    /// Get the latest report of this <see cref="User"/>'s profile by this <see cref="User"/>
    /// </summary>
    /// <param name="id">Reported <see cref="User"/> ID</param>
    /// <param name="reporterId"><see cref="User"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="Report"/> or null</returns>
    public Task<Report?> GetByUserAndReporterIdAsync(int id, int reporterId, CancellationToken cancellationToken) => 
        Set
            .AsQueryable()
            .OrderByDescending(r => r.DateTime)
            .FirstOrDefaultAsync(r => r.UserId == id && r.ReportingUserId == reporterId, cancellationToken);
    
    /// <summary>
    /// Get the latest report of this <see cref="Comment"/> by this <see cref="User"/>
    /// </summary>
    /// <param name="id"><see cref="Comment"/> ID</param>
    /// <param name="reporterId"><see cref="User"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="Report"/> or null</returns>
    public Task<Report?> GetByCommentAndReporterIdAsync(int id, int reporterId, CancellationToken cancellationToken)=> 
        Set
            .AsQueryable()
            .OrderByDescending(r => r.DateTime)
            .FirstOrDefaultAsync(r => r.CommentId == id && r.ReportingUserId == reporterId, cancellationToken);
    
    /// <summary>
    /// Get the latest report of this <see cref="Test"/> by this <see cref="AnonymousUser"/>
    /// </summary>
    /// <param name="id"><see cref="Test"/> ID</param>
    /// <param name="reporterId"><see cref="AnonymousUser"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="Report"/> or null</returns>
    public Task<Report?> GetByTestAndAnonReporterIdAsync(int id, Guid reporterId, CancellationToken cancellationToken) =>
        Set
            .AsQueryable()
            .OrderByDescending(r => r.DateTime)
            .FirstOrDefaultAsync(r => r.TestId == id && r.ReportingAnonymousUserId == reporterId, cancellationToken);
    
    /// <summary>
    /// Get the latest report of this <see cref="User"/> by this <see cref="AnonymousUser"/>
    /// </summary>
    /// <param name="id"><see cref="Comment"/> ID</param>
    /// <param name="reporterId"><see cref="AnonymousUser"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="Report"/> or null</returns>
    public Task<Report?> GetByUserAndAnonReporterIdAsync(int id, Guid reporterId, CancellationToken cancellationToken) =>
        Set
            .AsQueryable()
            .OrderByDescending(r => r.DateTime)
            .FirstOrDefaultAsync(r => r.UserId == id && r.ReportingAnonymousUserId == reporterId, cancellationToken);
    
    /// <summary>
    /// Get the latest report of this <see cref="Test"/> by this <see cref="AnonymousUser"/>
    /// </summary>
    /// <param name="id"><see cref="Test"/> ID</param>
    /// <param name="reporterId"><see cref="AnonymousUser"/> ID</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe</param>
    /// <returns><see cref="Report"/> or null</returns>
    public Task<Report?> GetByCommentAndAnonReporterIdAsync(int id, Guid reporterId, CancellationToken cancellationToken) =>
        Set
            .AsQueryable()
            .OrderByDescending(r => r.DateTime)
            .FirstOrDefaultAsync(r => r.CommentId == id && r.ReportingAnonymousUserId == reporterId, cancellationToken);
}